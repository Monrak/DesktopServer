using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Linq;
using DesktopServerManagerPro.Services;

namespace DesktopServerManagerPro;

public partial class MainForm : Form
{
    private string rootPath;
    private System.Windows.Forms.Timer statusTimer;
    private bool reallyExit = false;
    private string logFilePath;
    private SSLManager sslManager;

    public MainForm()
    {
        rootPath = AppDomain.CurrentDomain.BaseDirectory;
        // Robust check: Ensure we are in the right folder or a subfolder
        if (!Directory.Exists(Path.Combine(rootPath, "apache24")) && Directory.Exists(Path.Combine(rootPath, "..", "apache24")))
        {
            rootPath = Path.GetFullPath(Path.Combine(rootPath, ".."));
        }

        logFilePath = Path.Combine(rootPath, "manager.log");
        Log($"========================================");
        Log($"Manager Constructor started. Root path: {rootPath}");

        try
        {
            Log("Calling InitializeComponent...");
            InitializeComponent();

            // Load dynamic icon for tray if available
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_icon.ico");
            if (File.Exists(iconPath))
            {
                try
                {
                    using (var stream = new FileStream(iconPath, FileMode.Open, FileAccess.Read))
                    {
                        var dynIcon = new Icon(stream);
                        this.Icon = dynIcon;
                        notifyIcon.Icon = dynIcon;
                        Log($"Loaded custom icon from {iconPath}");
                    }
                }
                catch (Exception ex)
                {
                    Log($"Warning: Failed to load custom icon: {ex.Message}");
                }
            }
            else
            {
                notifyIcon.Icon = this.Icon;
            }

            CheckForIllegalCrossThreadCalls = false;
            Log("InitializeComponent completed.");
        }
        catch (Exception ex)
        {
            Log($"CRITICAL: InitializeComponent failed: {ex.Message}");
            Log($"Stack Trace: {ex.StackTrace}");
            MessageBox.Show($"UI Initialization Error: {ex.Message}", "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw; // Re-throw to be caught by Program.Main
        }

        LoadPersistentLogs();
        WireEvents();

        statusTimer = new System.Windows.Forms.Timer();
        statusTimer.Interval = 2000;
        statusTimer.Tick += StatusTimer_Tick;
        statusTimer.Start();

        this.FormClosing += MainForm_FormClosing;

        // Re-assign ContextMenu for reliability and wire events
        notifyIcon.ContextMenuStrip = trayMenu;
        menuShow.Click += (s, e) => RestoreFromTray();
        menuExit.Click += (s, e) => { reallyExit = true; Application.Exit(); };
        notifyIcon.DoubleClick += (s, e) => RestoreFromTray();

        // Explicitly show menu on right click if property assignment is flaky
        notifyIcon.MouseClick += (s, e) =>
        {
            if (e.Button == MouseButtons.Right)
            {
                trayMenu.Show(Cursor.Position);
            }
        };

        // Set up checkbox and auto-start
        chkStartWithWindows.Checked = IsRegisteredForStartup();
        chkStartWithWindows.CheckedChanged += (s, e) => SetStartup(chkStartWithWindows.Checked);

        // SSL Initialization
        sslManager = new SSLManager(Path.Combine(rootPath, "apache24"));
        InitializeSSLUI();

        // Start services automatically on load
        Task.Run(async () =>
        {
            await Task.Delay(1000); // Small delay to let UI stabilize
            if (this.IsHandleCreated) this.Invoke(new Action(() => InitPhpVersions()));
            AutoStartServices();
        });
    }

    private void InitPhpVersions()
    {
        cboPhpVersion.Items.Clear();
        // Detect available PHP versions
        var potentialVersions = new[] { "php74", "php56", "php82" };
        foreach (var ver in potentialVersions)
        {
            if (Directory.Exists(Path.Combine(rootPath, ver)))
            {
                cboPhpVersion.Items.Add(ver);
            }
        }

        // Fallback if none found, though php74 should exist
        if (cboPhpVersion.Items.Count == 0) cboPhpVersion.Items.Add("php74");

        // Try to detect current version from httpd.conf
        string current = DetectCurrentPhpVersion() ?? "php74";
        cboPhpVersion.SelectedItem = current;

        // Wire 'Set' button to switch version
        btnSwitchPhp.Click += async (s, e) =>
        {
            if (cboPhpVersion.SelectedItem != null)
                await SwitchPhpVersion(cboPhpVersion.SelectedItem.ToString());
        };
    }

    private string? DetectCurrentPhpVersion()
    {
        try
        {
            string confPath = Path.Combine(rootPath, @"apache24\conf\httpd.conf");
            if (!File.Exists(confPath)) return null;
            string content = File.ReadAllText(confPath);
            if (content.Contains("/php56/")) return "php56";
            if (content.Contains("/php82/")) return "php82";
            if (content.Contains("/php74/")) return "php74";
        }
        catch { }
        return null;
    }

    private async Task SwitchPhpVersion(string? newVersion)
    {
        if (string.IsNullOrEmpty(newVersion)) return;

        // Confirmation Dialog
        var result = MessageBox.Show($"Are you sure you want to switch to {newVersion}? \n\nApache will be restarted.",
            "Confirm Switch", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            // Revert combo box to previous version if needed, or just return
            // Ideally we should track 'previousVersion' to revert selection
            // For simplicity, we just reload the detected version from config
            string current = DetectCurrentPhpVersion() ?? "php74";
            cboPhpVersion.SelectedItem = current;
            return;
        }

        Log($"Switching to {newVersion}...");

        // 1. Stop Apache
        if (IsProcessRunning("httpd"))
        {
            ActionService("apache", "stop");
            await Task.Delay(2000); // Wait for stop
        }

        // 2. Update Config
        try
        {
            string confPath = Path.Combine(rootPath, @"apache24\conf\httpd.conf");
            string content = await File.ReadAllTextAsync(confPath);

            // Regex to replace PHPIniDir
            content = System.Text.RegularExpressions.Regex.Replace(content,
                @"PHPIniDir\s+"".*?""",
                $"PHPIniDir \"${{SRVROOT}}/../{newVersion}\"");

            // Regex to replace LoadModule
            // PHP 5.x uses php5_module, PHP 7.x uses php7_module, PHP 8.x uses php_module
            string dllName = "php7apache2_4.dll";
            string moduleName = "php7_module";

            if (newVersion.Contains("php5")) { dllName = "php5apache2_4.dll"; moduleName = "php5_module"; }
            else if (newVersion.Contains("php8")) { dllName = "php8apache2_4.dll"; moduleName = "php_module"; }

            // Replace the LoadModule line completely
            // We look for any LoadModule php*_module line
            content = System.Text.RegularExpressions.Regex.Replace(content,
                @"LoadModule\s+php\d?_module\s+"".*?""",
                $"LoadModule {moduleName} \"${{SRVROOT}}/../{newVersion}/{dllName}\"");

            await File.WriteAllTextAsync(confPath, content);
            Log($"Config updated for {newVersion}.");
        }
        catch (Exception ex)
        {
            Log($"Error updating config: {ex.Message}");
            MessageBox.Show("Failed to update Apache config. See log.");
            return;
        }

        // 3. Start Apache
        // 3. Start Apache
        ActionService("apache", "start");

        // 4. Auto-Open phpMyAdmin (Compatibility Check)
        try
        {
            string pmaSuffix = newVersion.Contains("php5") ? "phpmyadmin56" : "phpmyadmin";
            Log($"Auto-launching phpMyAdmin for {newVersion}...");

            // Notify user about the URL change if it's PHP 5
            if (newVersion.Contains("php5"))
            {
                MessageBox.Show("Using PHP 5.6: phpMyAdmin URL is changed to /phpmyadmin56 for compatibility.",
                                "Compatibility Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            Process.Start(new ProcessStartInfo($"http://localhost/{pmaSuffix}") { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            Log($"Failed to auto-launch browser: {ex.Message}");
        }
    }

    private void LoadPersistentLogs()
    {
        try
        {
            if (File.Exists(logFilePath))
            {
                string content = File.ReadAllText(logFilePath);
                txtLog.Text = content;
                txtLog.SelectionStart = txtLog.Text.Length;
                txtLog.ScrollToCaret();
            }
        }
        catch { /* Ignore log loading errors */ }
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (!reallyExit)
        {
            e.Cancel = true;
            this.Hide();
            // Balloon tip removed as requested
        }
    }

    private void RestoreFromTray()
    {
        this.Show();
        this.WindowState = FormWindowState.Normal;
        this.BringToFront();
    }

    private void WireEvents()
    {
        btnStartApache.Click += (s, e) => ActionService("apache", "start");
        btnStopApache.Click += (s, e) => ActionService("apache", "stop");
        btnStartMySQL.Click += (s, e) => ActionService("mysql", "start");
        btnStopMySQL.Click += (s, e) => ActionService("mysql", "stop");
        btnOpenWeb.Click += (s, e) =>
        {
            string protocol = chkEnableSSL.Checked ? "https" : "http";
            Process.Start(new ProcessStartInfo($"{protocol}://localhost") { UseShellExecute = true });
        };
        btnOpenPMA.Click += (s, e) =>
        {
            string currentVer = cboPhpVersion.SelectedItem?.ToString() ?? "php74";
            string pmaSuffix = currentVer.Contains("php5") ? "phpmyadmin56" : "phpmyadmin";
            string protocol = chkEnableSSL.Checked ? "https" : "http";
            Process.Start(new ProcessStartInfo($"{protocol}://localhost/{pmaSuffix}") { UseShellExecute = true });
        };

        // Config & Logs
        btnEditApacheConfig.Click += (s, e) => OpenFileWithNotepad(Path.Combine(rootPath, @"apache24\conf\httpd.conf"));
        btnOpenApacheLog.Click += (s, e) => OpenFileWithNotepad(Path.Combine(rootPath, @"apache24\logs\error.log"));
        btnEditMySQLConfig.Click += (s, e) => OpenFileWithNotepad(Path.Combine(rootPath, @"mysql80\my.ini"));
        btnEditPHPConfig.Click += (s, e) =>
        {
            string ver = cboPhpVersion.SelectedItem?.ToString() ?? "php74";
            OpenFileWithNotepad(Path.Combine(rootPath, $@"{ver}\php.ini"));
        };
        btnEditPMAConfig.Click += (s, e) =>
        {
            string currentVer = cboPhpVersion.SelectedItem?.ToString() ?? "php74";
            string pmaFolder = currentVer.Contains("php5") ? "phpmyadmin56" : "phpmyadmin";
            OpenFileWithNotepad(Path.Combine(rootPath, $@"www\{pmaFolder}\config.inc.php"));
        };
        btnOpenMySQLLog.Click += (s, e) => OpenMySQLLog();
        btnTerminal.Click += (s, e) => LaunchTerminal();
    }

    private void LaunchTerminal()
    {
        try
        {
            string currentPhp = cboPhpVersion.SelectedItem?.ToString() ?? "php74";
            string phpPath = Path.Combine(rootPath, currentPhp);
            string mysqlPath = Path.Combine(rootPath, @"mysql80\bin");

            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe");
            psi.UseShellExecute = false;
            psi.WorkingDirectory = Path.Combine(rootPath, "www");

            // Inject PATH
            string currentPath = Environment.GetEnvironmentVariable("Path") ?? "";
            psi.EnvironmentVariables["Path"] = $"{phpPath};{mysqlPath};{currentPath}";

            psi.Arguments = $"/k \"title DesktopServer Terminal [{currentPhp}] & echo Welcome to DesktopServer Terminal! & php -v\"";

            Process.Start(psi);
        }
        catch (Exception ex)
        {
            Log($"Failed to open terminal: {ex.Message}");
        }
    }

    private void OpenMySQLLog()
    {
        string dataDir = Path.Combine(rootPath, @"mysql80\data");
        if (Directory.Exists(dataDir))
        {
            var logFiles = Directory.GetFiles(dataDir, "*.err");
            if (logFiles.Length > 0)
            {
                // Open the most recently modified .err file
                var newestLog = logFiles.OrderByDescending(f => File.GetLastWriteTime(f)).First();
                OpenFileWithNotepad(newestLog);
                return;
            }
        }
        OpenFolder(dataDir);
    }

    private void OpenFileWithNotepad(string path)
    {
        if (File.Exists(path))
            Process.Start("notepad.exe", path);
        else
            MessageBox.Show($"File not found: {path}");
    }

    private void OpenFolder(string path)
    {
        if (Directory.Exists(path))
            Process.Start("explorer.exe", path);
        else
            MessageBox.Show($"Directory not found: {path}");
    }

    private void Log(string msg)
    {
        string logLine = $"[{DateTime.Now:HH:mm:ss}] {msg}";

        // 1. UI Logging (safe check)
        if (txtLog != null && !txtLog.IsDisposed)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => Log(msg)));
                return;
            }
            txtLog.AppendText(logLine + "\r\n");
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        // 2. File Logging (Manager log)
        try
        {
            if (!string.IsNullOrEmpty(logFilePath))
            {
                File.AppendAllText(logFilePath, logLine + Environment.NewLine);
            }
        }
        catch { }

        // 3. Startup Logging (Program log)
        Program.Log(msg);
    }

    private void StatusTimer_Tick(object? sender, EventArgs e)
    {
        UpdateStatus("apache", lblApacheStatus);
        UpdateStatus("mysql", lblMySQLStatus);
    }

    private void UpdateStatus(string service, Label label)
    {
        bool isRunning = IsProcessRunning(service == "apache" ? "httpd" : "mysqld");
        label.Text = isRunning ? "Running" : "Stopped";
        label.ForeColor = isRunning ? Color.Green : Color.Red;

        if (service == "apache")
        {
            btnStartApache.Enabled = !isRunning;
            btnStopApache.Enabled = isRunning;
        }
        else
        {
            btnStartMySQL.Enabled = !isRunning;
            btnStopMySQL.Enabled = isRunning;
        }
    }

    private bool IsProcessRunning(string name)
    {
        return Process.GetProcessesByName(name).Length > 0;
    }

    private void ActionService(string service, string action)
    {
        string binPath = "";
        string args = "";

        if (service == "apache")
        {
            binPath = Path.Combine(rootPath, @"apache24\bin\httpd.exe");
            if (!File.Exists(binPath)) { MessageBox.Show("Apache not found. Run Setup first."); return; }

            if (action == "start")
            {
                // Check if port 80 is in use
                if (IsPortInUse(80))
                {
                    Log("ERROR: Port 80 is already in use. Please close other web servers (like IIS or XAMPP).");
                    return;
                }
                args = ""; // Just run httpd.exe directly
            }
            else if (action == "stop")
            {
                StopProcess("httpd");
                return;
            }
        }
        else if (service == "mysql")
        {
            binPath = Path.Combine(rootPath, @"mysql80\bin\mysqld.exe");
            if (!File.Exists(binPath)) { MessageBox.Show("MySQL not found. Run Setup first."); return; }

            if (action == "start")
            {
                // Check if port 3306 is in use
                if (IsPortInUse(3306))
                {
                    Log("ERROR: Port 3306 is already in use. Please close other MySQL instances.");
                    return;
                }

                string dataDir = Path.Combine(rootPath, @"mysql80\data");
                if (!Directory.Exists(dataDir))
                {
                    Log("Initializing MySQL database...");
                    var initProcess = Process.Start(new ProcessStartInfo(binPath, "--initialize-insecure") { WindowStyle = ProcessWindowStyle.Hidden, CreateNoWindow = true });
                    initProcess?.WaitForExit();
                }
                args = $"--defaults-file=\"{rootPath}\\mysql80\\my.ini\" --console";
            }
            else if (action == "stop") { StopMySQL(); return; }
        }

        try
        {
            if (action == "start")
            {
                Log($"Starting {service}...");
                var psi = new ProcessStartInfo(binPath, args)
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = Path.GetDirectoryName(binPath)
                };

                // CRITICAL: Inject current PHP path into Apache's Environment PATH
                // This allows PHP extensions to find dependencies like libeay32.dll, ssleay32.dll
                if (service == "apache")
                {
                    string currentPhp = cboPhpVersion.SelectedItem?.ToString() ?? "php74";
                    string phpPath = Path.Combine(rootPath, currentPhp);
                    string existingPath = Environment.GetEnvironmentVariable("Path") ?? "";
                    psi.EnvironmentVariables["Path"] = $"{phpPath};{existingPath}";
                    Log($"Injecting {currentPhp} into Apache path...");
                }

                // MySQL Isolation Fix: Prepend MySQL bin to Path for this process only
                if (service == "mysql")
                {
                    string mysqlBin = Path.Combine(rootPath, @"mysql80\bin");
                    string currentPath = Environment.GetEnvironmentVariable("Path") ?? "";
                    psi.EnvironmentVariables["Path"] = $"{mysqlBin};{currentPath}";
                }

                var process = new Process { StartInfo = psi, EnableRaisingEvents = true };
                process.OutputDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) Log($"{service}: {e.Data}"); };
                process.ErrorDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) Log($"{service}: {e.Data}"); };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                Log($"{service} process launched.");

                // Auto-open localhost on FIRST RUN only
                if (service == "apache")
                {
                    string lockFile = Path.Combine(rootPath, "firstrun.lock");
                    if (!File.Exists(lockFile))
                    {
                        try
                        {
                            Log("First run detected. Opening localhost...");
                            Process.Start(new ProcessStartInfo("http://localhost") { UseShellExecute = true });
                            File.WriteAllText(lockFile, DateTime.Now.ToString());
                        }
                        catch (Exception ex) { Log($"Failed to open browser: {ex.Message}"); }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log($"Critical error {action}ing {service}: {ex.Message}");
        }
    }

    private void StopProcess(string name)
    {
        Log($"Stopping {name}...");
        foreach (var process in Process.GetProcessesByName(name))
        {
            try { process?.Kill(); Log($"{name} process killed."); } catch (Exception ex) { Log($"Failed to kill {name}: {ex.Message}"); }
        }
    }

    private void StopMySQL()
    {
        Log("Attempting graceful MySQL shutdown...");
        string adminPath = Path.Combine(rootPath, @"mysql80\bin\mysqladmin.exe");

        if (File.Exists(adminPath))
        {
            try
            {
                var psi = new ProcessStartInfo(adminPath, "-u root shutdown")
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                var proc = Process.Start(psi);
                if (proc != null)
                {
                    bool finished = proc.WaitForExit(5000); // Wait 5 seconds for graceful shutdown
                    if (finished)
                    {
                        Log("MySQL graceful shutdown command sent.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Graceful shutdown failed: {ex.Message}");
            }
        }

        Log("Falling back to process kill for MySQL...");
        StopProcess("mysqld");
    }

    private void AutoStartServices()
    {
        Log("Auto-starting services...");
        if (!IsProcessRunning("httpd"))
        {
            ActionService("apache", "start");
        }
        if (!IsProcessRunning("mysqld"))
        {
            ActionService("mysql", "start");
        }
    }

    private bool IsRegisteredForStartup()
    {
        try
        {
            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false))
            {
                return key?.GetValue("DesktopServerManager") != null;
            }
        }
        catch { return false; }
    }

    private void SetStartup(bool enable)
    {
        try
        {
            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                if (enable)
                {
                    key?.SetValue("DesktopServerManager", $"\"{Application.ExecutablePath}\"");
                    Log("Enabled 'Start with Windows'.");
                }
                else
                {
                    key?.DeleteValue("DesktopServerManager", false);
                    Log("Disabled 'Start with Windows'.");
                }
            }
        }
        catch (Exception ex)
        {
            Log($"Error updating startup registry: {ex.Message}");
        }
    }

    private bool IsPortInUse(int port)
    {
        try
        {
            System.Net.NetworkInformation.IPGlobalProperties ipGlobalProperties = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
            System.Net.IPEndPoint[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();
            foreach (System.Net.IPEndPoint endpoint in tcpConnInfoArray)
            {
                if (endpoint.Port == port) return true;
            }
        }
        catch { }
        return false;
    }

    private void btnEditPHPConfig_Click(object sender, EventArgs e)
    {

    }

    private async void InitializeSSLUI()
    {
        bool isEnabled = await sslManager.IsSSLEnabled();
        chkEnableSSL.Checked = isEnabled;
        btnTrustCert.Visible = isEnabled;

        chkEnableSSL.CheckedChanged += async (s, e) =>
        {
            chkEnableSSL.Enabled = false;
            if (chkEnableSSL.Checked)
            {
                Log("Enabling SSL/HTTPS...");
                bool success = await sslManager.SetupSSL();
                if (success)
                {
                    Log("SSL configured successfully. Please restart Apache to apply changes.");
                    btnTrustCert.Visible = true;
                    // Auto-restart Apache if running
                    if (IsProcessRunning("httpd"))
                    {
                        Log("Restarting Apache automatically...");
                        ActionService("apache", "stop");
                        await Task.Delay(1000);
                        ActionService("apache", "start");
                    }
                }
                else
                {
                    Log("FAILED to configure SSL. Check logs.");
                    chkEnableSSL.Checked = false;
                }
            }
            else
            {
                Log("Disabling SSL/HTTPS...");
                await sslManager.DisableSSL();
                Log("SSL disabled. Please restart Apache.");
                btnTrustCert.Visible = false;
                if (IsProcessRunning("httpd"))
                {
                    Log("Restarting Apache automatically...");
                    ActionService("apache", "stop");
                    await Task.Delay(1000);
                    ActionService("apache", "start");
                }
            }
            chkEnableSSL.Enabled = true;
        };

        btnTrustCert.Click += async (s, e) =>
        {
            Log("Attempting to trust Local SSL Certificate (Requires Admin)...");
            bool success = await sslManager.TrustCertificate();
            if (success)
            {
                Log("Certificate added to Trusted Root Authorities successfully!");
                MessageBox.Show("Certificate Trusted Successfully!\n\nYou may need to restart your browser to see the effect.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Log("Failed to trust certificate or user cancelled Admin prompt.");
            }
        };
    }

    private void menuAbout_Click(object sender, EventArgs e)
    {
        using (var about = new AboutForm())
        {
            about.ShowDialog();
        }
    }

    private void MainForm_Load(object sender, EventArgs e)
    {

    }
}

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace DesktopServerManager;

public partial class MainForm : Form
{
    private string rootPath;
    private System.Windows.Forms.Timer statusTimer;
    private bool reallyExit = false;
    private string logFilePath;
    private bool lastApacheRunning = false;

    public MainForm()
    {
        InitializeComponent();
        rootPath = AppDomain.CurrentDomain.BaseDirectory;
        // Robust check: Ensure we are in the right folder or a subfolder
        if (!Directory.Exists(Path.Combine(rootPath, "apache24")) && Directory.Exists(Path.Combine(rootPath, "..", "apache24")))
        {
            rootPath = Path.GetFullPath(Path.Combine(rootPath, ".."));
        }

        logFilePath = Path.Combine(rootPath, "manager.log");
        Log($"Manager started. Root path: {rootPath}");

        // Load dynamic icon for tray if available
        string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "app_icon.ico");
        if (!File.Exists(iconPath)) iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_icon.ico");

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
            catch (Exception ex) { Log($"Warning: Failed to load custom icon: {ex.Message}"); }
        }
        else
        {
            notifyIcon.Icon = this.Icon;
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

        // Start services automatically on load
        Task.Run(async () =>
        {
            await Task.Delay(1000); // Small delay to let UI stabilize
            AutoStartServices();
        });
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
        btnOpenWeb.Click += (s, e) => Process.Start(new ProcessStartInfo("http://localhost") { UseShellExecute = true });
        btnOpenPMA.Click += (s, e) => Process.Start(new ProcessStartInfo("http://localhost/phpmyadmin") { UseShellExecute = true });

        // Config & Logs
        btnEditApacheConfig.Click += (s, e) => OpenFileWithNotepad(Path.Combine(rootPath, @"apache24\conf\httpd.conf"));
        btnOpenApacheLog.Click += (s, e) => OpenFileWithNotepad(Path.Combine(rootPath, @"apache24\logs\error.log"));
        btnEditMySQLConfig.Click += (s, e) => OpenFileWithNotepad(Path.Combine(rootPath, @"mysql80\my.ini"));
        btnEditPHPConfig.Click += (s, e) => OpenFileWithNotepad(Path.Combine(rootPath, @"php74\php.ini"));
        btnEditPMAConfig.Click += (s, e) => OpenFileWithNotepad(Path.Combine(rootPath, @"www\phpmyadmin\config.inc.php"));
        btnOpenMySQLLog.Click += (s, e) => OpenMySQLLog();
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
        if (txtLog.IsDisposed) return;
        if (txtLog.InvokeRequired)
        {
            txtLog.Invoke(new Action(() => Log(msg)));
            return;
        }
        string logLine = $"[{DateTime.Now:HH:mm:ss}] {msg}";
        txtLog.AppendText(logLine + "\r\n");
        txtLog.SelectionStart = txtLog.Text.Length;
        txtLog.ScrollToCaret();

        try
        {
            File.AppendAllText(logFilePath, logLine + Environment.NewLine);
        }
        catch { /* Ignore log saving errors */ }
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

            if (isRunning && !lastApacheRunning)
            {
                Task.Run(() => {
                    Process.Start(new ProcessStartInfo("http://localhost/index.html") { UseShellExecute = true });
                });
            }
            lastApacheRunning = isRunning;
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
                    RedirectStandardOutput = true
                };

                var process = new Process { StartInfo = psi, EnableRaisingEvents = true };
                process.OutputDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) Log($"{service}: {e.Data}"); };
                process.ErrorDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) Log($"{service}: {e.Data}"); };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                Log($"{service} process launched.");
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

    private void aboutUsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        AboutForm about = new AboutForm();
        about.ShowDialog();
    }
}

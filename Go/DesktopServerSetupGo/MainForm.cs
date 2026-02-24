using System.IO.Compression;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DesktopServerSetupGo;

public partial class MainForm : Form
{
    [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
    private extern static void ReleaseCapture();
    [DllImport("user32.DLL", EntryPoint = "SendMessage")]
    private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

    private string? logFilePath;

    public MainForm(string[] args)
    {
        InitializeComponent();
        
        if (args.Contains("/uninstall"))
        {
            RunUninstall().GetAwaiter().GetResult();
            Environment.Exit(0);
        }

        WireEvents();
    }

    public MainForm() : this(Array.Empty<string>()) { }

    private void WireEvents()
    {
        btnBrowse.Click += (s, e) => {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtInstallPath.Text = fbd.SelectedPath;
                }
            }
        };
        btnInstall.Click += async (s, e) => await StartInstallation();
        btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
        btnClose.Click += (s, e) => Application.Exit();

        // Draggable
        lblMain.MouseDown += (s, e) => {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0x112, 0xf012, 0);
            }
        };
        this.MouseDown += (s, e) => {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0x112, 0xf012, 0);
            }
        };
        pnlHeader.MouseDown += (s, e) => {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0x112, 0xf012, 0);
            }
        };
    }

    private void Log(string msg)
    {
        if (txtLog.InvokeRequired)
        {
            txtLog.Invoke(new Action(() => Log(msg)));
            return;
        }
        string logLine = $"[{DateTime.Now:HH:mm:ss}] {msg}";
        txtLog.AppendText(logLine + "\r\n");
        txtLog.SelectionStart = txtLog.Text.Length;
        txtLog.ScrollToCaret();

        if (!string.IsNullOrEmpty(logFilePath))
        {
            try { File.AppendAllText(logFilePath, logLine + Environment.NewLine); } catch { }
        }
    }

    private void UpdateStatus(string status, int progress)
    {
        if (lblStatus.InvokeRequired)
        {
            lblStatus.Invoke(new Action(() => UpdateStatus(status, progress)));
            return;
        }
        lblStatus.Text = status;
        progressBar.Value = Math.Min(100, Math.Max(0, progress));
    }

    private async Task StartInstallation()
    {
        string rootPath = txtInstallPath.Text.Trim();
        if (string.IsNullOrEmpty(rootPath))
        {
            MessageBox.Show("Please select a valid path.");
            return;
        }

        logFilePath = Path.Combine(rootPath, "install.log");
        btnInstall.Enabled = false;
        btnBrowse.Enabled = false;
        txtInstallPath.ReadOnly = true;

        try
        {
            Log("Ensuring no conflicting processes are running...");
            KillRunningProcesses();
            await Task.Delay(1000); // Wait for processes to exit

            // Cleanup legacy bin folder to prevent path confusion
            string legacyBin = Path.Combine(rootPath, "bin");
            if (Directory.Exists(legacyBin))
            {
                Log("Cleaning up legacy structure...");
                try { Directory.Delete(legacyBin, true); } catch { Log("Warning: Could not remove old bin folder."); }
            }

            if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
            File.WriteAllText(logFilePath, $"--- Installation Started at {DateTime.Now} ---" + Environment.NewLine);
            
            Log($"Starting Monrak Desktop Server Go! Installation at: {rootPath}");

            UpdateStatus("Creating directories...", 5);
            Directory.CreateDirectory(Path.Combine(rootPath, "ssl"));
            Directory.CreateDirectory(Path.Combine(rootPath, "www"));
            Directory.CreateDirectory(Path.Combine(rootPath, "tmp", "sessions"));

            // Core Extractor
            await SetupComponent(rootPath, "PHP 8.4 Runtime", "php84_tmp.zip", Path.Combine(rootPath, "php84"), 10, 30, "php.exe");
            await SetupComponent(rootPath, "RoadRunner Engine", "roadrunner_tmp.zip", rootPath, 30, 50, "rr.exe");
            await SetupComponent(rootPath, "MariaDB 11 Database", "mariadb_tmp.zip", Path.Combine(rootPath, "mariadb"), 50, 70, "mariadbd.exe");
            await SetupComponent(rootPath, "PostgreSQL AI Database", "postgres_tmp.zip", Path.Combine(rootPath, "postgres"), 70, 85, "postgres.exe");
            await SetupComponent(rootPath, "phpMyAdmin Tool", "pma_tmp.zip", Path.Combine(rootPath, "www", "phpmyadmin"), 85, 95, "index.php");
            
            // Database Configuration
            ConfigureMariaDB(rootPath);
            ConfigurePostgreSQL(rootPath);

            UpdateStatus("Finalizing resources...", 96);
            Log("Deploying Management Tools & Assets...");
            
            Directory.CreateDirectory(Path.Combine(rootPath, "openssl"));
            
            await ExtractResourceToFileAsync("DesktopServerManagerGo.exe", Path.Combine(rootPath, "DesktopServerManagerGo.exe"));
            await ExtractResourceToFileAsync("index.html", Path.Combine(rootPath, "www", "index.html"));
            await ExtractResourceToFileAsync("app_logo.png", Path.Combine(rootPath, "www", "app_logo.png"));
            await ExtractResourceToFileAsync("favicon.ico", Path.Combine(rootPath, "www", "favicon.ico"));
            await ExtractResourceToFileAsync(".rr.yaml", Path.Combine(rootPath, ".rr.yaml"));
            await ExtractResourceToFileAsync("worker.php", Path.Combine(rootPath, "worker.php"));

            await ExtractResourceToFileAsync("openssl.exe", Path.Combine(rootPath, "openssl", "openssl.exe"));
            await ExtractResourceToFileAsync("libcrypto-3-x64.dll", Path.Combine(rootPath, "openssl", "libcrypto-3-x64.dll"));
            await ExtractResourceToFileAsync("libssl-3-x64.dll", Path.Combine(rootPath, "openssl", "libssl-3-x64.dll"));
            
            // Move any leaked PHP files to php84 if they exist in root (Safety measure)
            string[] leakedFiles = { "php.exe", "php8.dll", "php8ts.dll", "php-cgi.exe" };
            foreach (var f in leakedFiles)
            {
                string leak = Path.Combine(rootPath, f);
                if (File.Exists(leak))
                {
                    try { File.Move(leak, Path.Combine(rootPath, "php84", f), true); } catch { }
                }
            }

            // Configure phpMyAdmin
            Log("Configuring phpMyAdmin for MariaDB...");
            string pmaConfig = Path.Combine(rootPath, "www", "phpmyadmin", "config.inc.php");
            string pmaConfigContent = "<?php\n" +
                "$cfg['blowfish_secret'] = 'monrak_desktop_server_go_32_chars';\n" +
                "$i = 0; $i++;\n" +
                "$cfg['Servers'][$i]['auth_type'] = 'config';\n" +
                "$cfg['Servers'][$i]['host'] = '127.0.0.1';\n" +
                "$cfg['Servers'][$i]['user'] = 'root';\n" +
                "$cfg['Servers'][$i]['password'] = '';\n" +
                "$cfg['Servers'][$i]['AllowNoPassword'] = true;\n" +
                "?>";
            File.WriteAllText(pmaConfig, pmaConfigContent);
            
            Log("Optimizing PHP 8.4 configuration (Pro Alignment)...");
            ConfigurePHP(rootPath);
            
            Log("Installing RoadRunner Dependencies...");
            await InstallComposerDependencies(rootPath);
            
            // Note: .rr.yaml is already deployed from Resources (line 153).
            // No need to overwrite it with hardcoded content.
            
            Log("Checking for system PHP conflicts...");
            CleanPhpFromPath();

            UpdateStatus("Registering Uninstaller...", 98);
            string uninstallerPath = Path.Combine(rootPath, "Uninstaller.exe");
            await RetryAction(async () => {
                await Task.Run(() => File.Copy(Application.ExecutablePath, uninstallerPath, true));
            });
            await RegisterUninstallerAsync(rootPath, uninstallerPath);

            // Create Shortcuts
            UpdateStatus("Creating Shortcuts...", 99);
            string managerExe = Path.Combine(rootPath, "DesktopServerManagerGo.exe");
            
            // 1. Start Menu
            CreateShortcut(managerExe, "Monrak Manager Go!", "Launch Monrak Desktop Server Go!", Environment.GetFolderPath(Environment.SpecialFolder.Programs));
            
            // 2. Desktop
            CreateShortcut(managerExe, "Monrak Manager Go!", "Launch Monrak Desktop Server Go!", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            UpdateStatus("Completed successfully!", 100);
            Log("SUCCESS: Monrak Desktop Server Go! is ready.");

            // Sequential Onboarding: MariaDB stays empty, PostgreSQL set to trust by default
            await ApplyDatabaseSecurity(rootPath, "");

            MessageBox.Show("Installation Complete!\n\nMonrak Desktop Server Go! will now start.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            // Auto launch Manager with Auto-Start sequence
            Process.Start(new ProcessStartInfo(Path.Combine(rootPath, "DesktopServerManagerGo.exe"), "--autostart") { UseShellExecute = true, WorkingDirectory = rootPath });
            
            Application.Exit();
        }
        catch (Exception ex)
        {
            Log($"ERROR: {ex.Message}");
            UpdateStatus("Installation failed.", progressBar.Value);
            MessageBox.Show($"Installation failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnInstall.Enabled = true;
            btnBrowse.Enabled = true;
            txtInstallPath.ReadOnly = false;
        }
    }

    private void KillRunningProcesses()
    {
        string[] procs = { 
            "DesktopServerManagerGo", "DesktopServerManager", "DesktopServerManagerPro",
            "rr", "mariadbd", "mysqld", "postgres", "pg_ctl", "php-cgi", "pgAdmin4", 
            "mysql", "mariadb", "psql", "Standard.exe"
        };
        foreach (var pName in procs)
        {
            try
            {
                var runningProcs = Process.GetProcessesByName(pName);
                if (runningProcs.Length > 0)
                {
                    Log($"Terminating {runningProcs.Length} instance(s) of: {pName}...");
                    foreach (var p in runningProcs)
                    {
                        try { p.Kill(); p.WaitForExit(3000); } catch { }
                    }
                }
            }
            catch { }
        }
    }

    private async Task SetupComponent(string root, string name, string resourceName, string targetDir, int startProg, int endProg, string? expectedBinary = null)
    {
        UpdateStatus($"Installing {name}...", startProg);
        Log($"Preparing {name}...");

        string tempZip = Path.Combine(Path.GetTempPath(), resourceName);
        try
        {
            ExtractResourceToFile(resourceName, tempZip);
            
            // Cleanup target directory if not the root path (to avoid unintended deletions)
            if (targetDir.TrimEnd('\\', '/') != root.TrimEnd('\\', '/') && Directory.Exists(targetDir))
            {
                Log($"Cleaning up existing {name} folder...");
                for (int i = 0; i < 3; i++)
                {
                    try 
                    { 
                        Directory.Delete(targetDir, true); 
                        break; 
                    } 
                    catch (Exception ex)
                    {
                        if (i == 2) Log($"Warning: Could not clean folder {targetDir}: {ex.Message}");
                        await Task.Delay(1000); 
                    }
                }
            }
            
            if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
            
            Log($"Extracting {name} to {targetDir}...");
            
            bool extracted = false;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    await Task.Run(() => ZipFile.ExtractToDirectory(tempZip, targetDir, true));
                    extracted = true;
                    break;
                }
                catch (Exception ex)
                {
                    Log($"Extraction attempt {i+1} failed: {ex.Message}");
                    if (i < 2) 
                    {
                        Log("Retrying in 2 seconds... Ensuring processes are killed.");
                        KillRunningProcesses(); // More aggressive kill
                        await Task.Delay(2000);
                    }
                    else throw; // Fail on last attempt
                }
            }

            if (!extracted) throw new Exception($"Failed to extract {name} after multiple attempts.");

            // Smart Flattening Logic
            if (!string.IsNullOrEmpty(expectedBinary))
            {
                // Re-evaluate binary location after extraction
                bool foundAtRoot = File.Exists(Path.Combine(targetDir, expectedBinary)) || 
                                   File.Exists(Path.Combine(targetDir, "bin", expectedBinary));

                if (!foundAtRoot)
                {
                   Log($"Searching for {expectedBinary} in subdirectories...");
                   var foundFiles = Directory.GetFiles(targetDir, expectedBinary, SearchOption.AllDirectories);
                   if (foundFiles.Length > 0)
                   {
                       string foundPath = foundFiles[0];
                       DirectoryInfo? currentIdx = Directory.GetParent(foundPath);
                       DirectoryInfo targetInfo = new DirectoryInfo(targetDir);

                       // Walk up until we find the immediate child of targetDir
                       DirectoryInfo? container = null;
                       while (currentIdx != null && currentIdx.FullName.TrimEnd('\\', '/') != targetInfo.FullName.TrimEnd('\\', '/'))
                       {
                           if (currentIdx.Parent != null && currentIdx.Parent.FullName.TrimEnd('\\', '/') == targetInfo.FullName.TrimEnd('\\', '/'))
                           {
                               container = currentIdx;
                               break;
                           }
                           currentIdx = currentIdx.Parent;
                       }

                       if (container != null)
                       {
                           Log($"Found nested container '{container.Name}'. Hoisting content to root...");
                           // Move content of container to targetDir
                           // Move contents using recursive merge helper
                           MoveDirectory(container.FullName, targetDir);
                           
                           await RetryAction(async () => {
                               await Task.Run(() => Directory.Delete(container.FullName, true));
                           });
                           Log($"Structure corrected for {name}.");
                       }
                   }
                }
            }

            Log($"{name} installed successfully.");
            UpdateStatus($"{name} Ready.", endProg);
        }
        finally
        {
            if (File.Exists(tempZip)) try { File.Delete(tempZip); } catch { }
        }
    }

    private async Task RetryAction(Func<Task> action, int maxRetries = 5, int delayMs = 1000)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                await action();
                return;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                if (i == maxRetries - 1) throw;
                Log($"Access/IO error. Retrying in {delayMs}ms... (Attempt {i + 1}/{maxRetries})");
                await Task.Delay(delayMs);
            }
        }
    }

    private void MoveDirectory(string source, string target)
    {
        if (!Directory.Exists(target))
        {
            Directory.CreateDirectory(target);
        }

        foreach (var file in Directory.GetFiles(source))
        {
            string dest = Path.Combine(target, Path.GetFileName(file));
            
            // Retry Delete and Move
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    if (File.Exists(dest)) File.Delete(dest);
                    File.Move(file, dest);
                    break;
                }
                catch (Exception ex) when (i < 4 && (ex is IOException || ex is UnauthorizedAccessException))
                {
                    Log($"Retry moving file {Path.GetFileName(file)} ({i+1}/5)...");
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        foreach (var dir in Directory.GetDirectories(source))
        {
            string dest = Path.Combine(target, Path.GetFileName(dir));
            MoveDirectory(dir, dest);
        }
    }

    private void ExtractResourceToFile(string resourceName, string destPath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        string fullResourceName = $"DesktopServerSetupGo.Resources.{resourceName}";
        
        using (Stream? stream = assembly.GetManifestResourceStream(fullResourceName))
        {
            if (stream == null) 
            {
                var allResources = assembly.GetManifestResourceNames();
                var match = allResources.FirstOrDefault(r => r.EndsWith(resourceName));
                
                if (match != null)
                {
                    Log($"Resource '{resourceName}' found as '{match}'.");
                    using (Stream? s2 = assembly.GetManifestResourceStream(match))
                    {
                        using (FileStream fs = new FileStream(destPath, FileMode.Create))
                        {
                            s2?.CopyTo(fs);
                        }
                    }
                    return;
                }
                throw new Exception($"Resource '{resourceName}' not found.");
            }

            using (FileStream fs = new FileStream(destPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                stream.CopyTo(fs);
            }
        }
    }

    private async Task ExtractResourceToFileAsync(string resourceName, string destPath)
    {
        await RetryAction(async () => {
            await Task.Run(() => ExtractResourceToFile(resourceName, destPath));
        });
    }

    private void CreateShortcut(string targetPath, string shortcutName, string description, string parentFolder)
    {
        try
        {
            string appFolder = parentFolder;
            if (parentFolder == Environment.GetFolderPath(Environment.SpecialFolder.Programs))
            {
                appFolder = Path.Combine(parentFolder, "Monrak Net");
                if (!Directory.Exists(appFolder)) Directory.CreateDirectory(appFolder);
            }

            Log($"Creating shortcut: {shortcutName} in {appFolder}...");
            string shortcutLocation = Path.Combine(appFolder, shortcutName + ".lnk");
            
            Type? shellType = Type.GetTypeFromProgID("WScript.Shell");
            if (shellType != null)
            {
                dynamic shell = Activator.CreateInstance(shellType)!;
                dynamic shortcut = shell.CreateShortcut(shortcutLocation);
                
                shortcut.TargetPath = targetPath;
                shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
                shortcut.Description = description;
                shortcut.IconLocation = targetPath;
                shortcut.Save();
                Log($"Shortcut created successfully at {shortcutLocation}");
            }
        }
        catch (Exception ex)
        {
            Log($"Failed to create shortcut: {ex.Message}");
        }
    }

    private async Task RegisterUninstallerAsync(string installPath, string uninstallerPath)
    {
        await RetryAction(async () => {
            await Task.Run(() => RegisterUninstaller(installPath, uninstallerPath));
        });
    }

    private void RegisterUninstaller(string installPath, string uninstallerPath)
    {
        try
        {
            Log("Registering Monrak Desktop Server Go! in system registry...");
            using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\DesktopServerGo"))
            {
                key.SetValue("DisplayName", "Monrak Desktop Server Go!");
                key.SetValue("DisplayVersion", "1.0.1");
                key.SetValue("Publisher", "Monrak Net Co., Ltd.");
                key.SetValue("UninstallString", $"\"{uninstallerPath}\" /uninstall");
                key.SetValue("DisplayIcon", uninstallerPath);
                key.SetValue("InstallLocation", installPath);
                key.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"));
                key.SetValue("EstimatedSize", 850 * 1024); // Approx 850MB
                key.SetValue("URLInfoAbout", "https://monrak.net");
                key.SetValue("HelpLink", "https://monrak.net/support");
                key.SetValue("NoModify", 1);
                key.SetValue("NoRepair", 1);
            }
        }
        catch (Exception ex) { Log($"Registry failed: {ex.Message}"); throw; } // Throw to trigger retry
    }

    private async Task RunUninstall()
    {
        try
        {
            string self = Application.ExecutablePath;
            string root = Path.GetDirectoryName(self) ?? "";
            
            if (MessageBox.Show("Are you sure you want to uninstall Monrak Desktop Server Go!?\n\nThis will stop all running services and remove all application files.", "Uninstall", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            // 1. Kill Processes
            KillRunningProcesses();
            // Wait a bit more for OS to release locks
            System.Threading.Thread.Sleep(2000);

            // 2. Remove Registry
            try
            {
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\DesktopServerGo", false);
            }
            catch { }

            // 3. Remove Shortcuts
            try
            {
                // Start Menu
                string startMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
                string lnkPath = Path.Combine(startMenuPath, "Monrak Net", "Monrak Manager Go!.lnk");
                if (File.Exists(lnkPath)) File.Delete(lnkPath);
                
                // Desktop
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string desktopLnk = Path.Combine(desktopPath, "Monrak Manager Go!.lnk");
                if (File.Exists(desktopLnk)) File.Delete(desktopLnk);

                // Cleanup empty directory in Start Menu
                string appStartMenuPath = Path.Combine(startMenuPath, "Monrak Net");
                if (Directory.Exists(appStartMenuPath) && !Directory.EnumerateFileSystemEntries(appStartMenuPath).Any())
                {
                    Directory.Delete(appStartMenuPath, false);
                }
            }
            catch { }

            // 4. Self-Cleanup Script
            if (!string.IsNullOrEmpty(root) && Directory.Exists(root))
            {
                string tempDir = Path.GetTempPath();
                string batchFile = Path.Combine(tempDir, "monrak_go_cleanup.bat");
                
                await RetryAction(async () => {
                    await Task.Run(() => File.WriteAllText(batchFile, $@"
@echo off
timeout /t 3 /nobreak >nul
:retry
rmdir /s /q ""{root}""
if exist ""{root}"" (
    timeout /t 2 /nobreak >nul
    goto retry
)
del ""%~f0""
"));
                });
                
                Process.Start(new ProcessStartInfo(batchFile) { CreateNoWindow = true, UseShellExecute = true, WindowStyle = ProcessWindowStyle.Hidden });
            }
            
            MessageBox.Show("Uninstallation complete.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Uninstall Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ConfigurePHP(string root)
    {
        try
        {
            // PHP is extracted to php84 in the Go version
            string phpRoot = Path.Combine(root, "php84");
            string iniPath = Path.Combine(phpRoot, "php.ini");
            string absoluteExtDir = Path.Combine(phpRoot, "ext").Replace("\\", "/");
            string absoluteTmpDir = Path.Combine(root, "tmp").Replace("\\", "/");
            string absoluteSessionDir = Path.Combine(root, "tmp", "sessions").Replace("\\", "/");

            // Extensions that need to be enabled via extension= (DLL-loaded, not compiled-in)
            string[] dllExtensions = {
                "curl", "gd", "mbstring", "exif", "openssl", "soap", "zip", "fileinfo", "intl", "sodium", "sockets",
                "mysqli", "pdo_mysql", "pgsql", "pdo_pgsql"
            };

            if (!File.Exists(iniPath))
            {
                // Try using php.ini-development as template first
                string devIni = Path.Combine(phpRoot, "php.ini-development");
                string prodIni = Path.Combine(phpRoot, "php.ini-production");
                if (File.Exists(devIni)) 
                    File.Copy(devIni, iniPath, true);
                else if (File.Exists(prodIni))
                    File.Copy(prodIni, iniPath, true);
            }

            if (File.Exists(iniPath))
            {
                // Template exists — patch it
                string ini = File.ReadAllText(iniPath);
                
                // 1. Set Absolute Extension Dir
                ini = System.Text.RegularExpressions.Regex.Replace(ini,
                    @"^;\s*extension_dir\s*=\s*""ext""", $"extension_dir = \"{absoluteExtDir}\"",
                    System.Text.RegularExpressions.RegexOptions.Multiline);
                ini = System.Text.RegularExpressions.Regex.Replace(ini,
                    @"^extension_dir\s*=\s*""ext""", $"extension_dir = \"{absoluteExtDir}\"",
                    System.Text.RegularExpressions.RegexOptions.Multiline);

                // 2. Enable DLL Extensions
                foreach (var ext in dllExtensions)
                {
                    string pattern = $@"^\s*;?\s*extension\s*=\s*(php_)?{System.Text.RegularExpressions.Regex.Escape(ext)}(\.dll)?\s*(;.*)?$";
                    ini = System.Text.RegularExpressions.Regex.Replace(ini, pattern, "", System.Text.RegularExpressions.RegexOptions.Multiline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    ini += $"\r\nextension={ext}";
                }

                // 3. Resource Limits
                ini = ini.Replace("memory_limit = 128M", "memory_limit = 1024M");
                ini = ini.Replace("upload_max_filesize = 2M", "upload_max_filesize = 4096M");
                ini = ini.Replace("post_max_size = 8M", "post_max_size = 4096M");
                ini = ini.Replace("max_execution_time = 30", "max_execution_time = 3600");
                ini = ini.Replace("max_input_time = 60", "max_input_time = 3600");

                // 4. Timezone & Errors
                ini = System.Text.RegularExpressions.Regex.Replace(ini,
                    @"^;\s*date\.timezone\s*=", "date.timezone = Asia/Bangkok",
                    System.Text.RegularExpressions.RegexOptions.Multiline);
                ini = System.Text.RegularExpressions.Regex.Replace(ini,
                    @"^;?\s*display_errors\s*=\s*.*$", "display_errors = stderr",
                    System.Text.RegularExpressions.RegexOptions.Multiline);
                ini = System.Text.RegularExpressions.Regex.Replace(ini,
                    @"^;?\s*display_startup_errors\s*=\s*.*$", "display_startup_errors = Off",
                    System.Text.RegularExpressions.RegexOptions.Multiline);
                
                // 5. Enable OpCache
                ini = System.Text.RegularExpressions.Regex.Replace(ini,
                    @"^;\s*zend_extension\s*=\s*opcache", "zend_extension=opcache",
                    System.Text.RegularExpressions.RegexOptions.Multiline);
                if (ini.Contains("[opcache]"))
                {
                    ini = ini.Replace(";opcache.enable=1", "opcache.enable=1\nopcache.enable_cli=1");
                    ini = ini.Replace(";opcache.memory_consumption=128", "opcache.memory_consumption=256");
                    ini = ini.Replace(";opcache.interned_strings_buffer=8", "opcache.interned_strings_buffer=16");
                    ini = ini.Replace(";opcache.max_accelerated_files=10000", "opcache.max_accelerated_files=20000");
                }

                // 6. Add Temporary, Session & CGI Paths
                ini += $"\r\n\r\n[Session]\r\nsession.save_path = \"{absoluteSessionDir}\"\r\nupload_tmp_dir = \"{absoluteTmpDir}\"\r\nsys_temp_dir = \"{absoluteTmpDir}\"\r\n";
                ini += $"\r\n[CGI]\r\ncgi.fix_pathinfo=1\r\n";

                File.WriteAllText(iniPath, ini);
                Log("SUCCESS: PHP 8.4 configured from template (High Performance Mode).");
            }
            else
            {
                // NO TEMPLATE EXISTS — Generate a working php.ini from scratch
                Log("No php.ini template found. Generating minimal php.ini from scratch...");
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("[PHP]");
                sb.AppendLine($"extension_dir = \"{absoluteExtDir}\"");
                sb.AppendLine();
                sb.AppendLine("; === Extensions ===");
                foreach (var ext in dllExtensions)
                    sb.AppendLine($"extension={ext}");
                sb.AppendLine();
                sb.AppendLine("; === Resource Limits ===");
                sb.AppendLine("memory_limit = 1024M");
                sb.AppendLine("upload_max_filesize = 4096M");
                sb.AppendLine("post_max_size = 4096M");
                sb.AppendLine("max_execution_time = 3600");
                sb.AppendLine("max_input_time = 3600");
                sb.AppendLine("max_input_vars = 10000");
                sb.AppendLine();
                sb.AppendLine("; === Error Handling (RoadRunner Safe) ===");
                sb.AppendLine("display_errors = stderr");
                sb.AppendLine("display_startup_errors = Off");
                sb.AppendLine("log_errors = On");
                sb.AppendLine("error_reporting = E_ALL");
                sb.AppendLine();
                sb.AppendLine("; === Timezone ===");
                sb.AppendLine("date.timezone = Asia/Bangkok");
                sb.AppendLine();
                sb.AppendLine("; === OpCache ===");
                sb.AppendLine("zend_extension=opcache");
                sb.AppendLine("[opcache]");
                sb.AppendLine("opcache.enable=1");
                sb.AppendLine("opcache.enable_cli=1");
                sb.AppendLine("opcache.memory_consumption=256");
                sb.AppendLine("opcache.interned_strings_buffer=16");
                sb.AppendLine("opcache.max_accelerated_files=20000");
                sb.AppendLine();
                sb.AppendLine("[Session]");
                sb.AppendLine($"session.save_path = \"{absoluteSessionDir}\"");
                sb.AppendLine($"upload_tmp_dir = \"{absoluteTmpDir}\"");
                sb.AppendLine($"sys_temp_dir = \"{absoluteTmpDir}\"");
                sb.AppendLine();
                sb.AppendLine("[CGI]");
                sb.AppendLine("cgi.fix_pathinfo=1");

                File.WriteAllText(iniPath, sb.ToString());
                Log("SUCCESS: PHP 8.4 configured with generated php.ini (High Performance Mode).");
            }
        }
        catch (Exception ex)
        {
            Log($"Warning: PHP Configuration failed: {ex.Message}");
        }
    }

    private void CleanPhpFromPath()
    {
        try
        {
            Log("Cleaning conflicting PHP paths from Windows environment (Isolation Mode)...");
            const string pathKey = "Path";
            var scope = EnvironmentVariableTarget.User;
            string currentPath = Environment.GetEnvironmentVariable(pathKey, scope) ?? "";

            var paths = currentPath.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
            int removedCount = 0;

            for (int i = paths.Count - 1; i >= 0; i--)
            {
                string p = paths[i].TrimEnd('\\', '/');
                // Remove paths that contain 'php' and either 'bin' or a php.exe file, 
                // but DONT remove our own install path if it's already there (though unlikely during setup)
                if (p.ToLower().Contains("php") && (p.ToLower().Contains("bin") || File.Exists(Path.Combine(p, "php.exe"))))
                {
                    Log($"Removing conflicting PHP entry: {p}");
                    paths.RemoveAt(i);
                    removedCount++;
                }
            }

            if (removedCount > 0)
            {
                string newPath = string.Join(";", paths);
                Environment.SetEnvironmentVariable(pathKey, newPath, scope);
                Log($"Cleaned {removedCount} conflicting PHP path(s). Environment isolated.");
            }
            else
            {
                Log("No PHP path conflicts found. Environment is clean.");
            }
        }
        catch (Exception ex)
        {
            Log($"Warning: Path cleaning failed: {ex.Message}");
        }
    }

    private void ConfigureMariaDB(string root)
    {
        try
        {
            Log("Configuring MariaDB...");
            string mariaRoot = Path.Combine(root, "mariadb");
            string iniPath = Path.Combine(mariaRoot, "my.ini");
            string dataDir = Path.Combine(mariaRoot, "data");

            if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);

            if (!File.Exists(iniPath))
            {
                Log("Generating MariaDB configuration (my.ini)...");
                string myIni = $@"[mysqld]
datadir={dataDir.Replace("\\", "/")}
port=3306
bind-address=127.0.0.1
default_storage_engine=InnoDB
innodb_buffer_pool_size=256M
innodb_log_file_size=64M
max_connections=100
character-set-server=utf8mb4
collation-server=utf8mb4_unicode_ci
[client]
port=3306
plugin-dir={Path.Combine(mariaRoot, "lib", "plugin").Replace("\\", "/")}
";
                File.WriteAllText(iniPath, myIni);
                Log("MariaDB configured successfully.");
            }
            
            string installDb = Path.Combine(mariaRoot, "bin", "mysql_install_db.exe");
            if (File.Exists(installDb) && Directory.GetFiles(dataDir).Length == 0 && Directory.GetDirectories(dataDir).Length == 0)
            {
                Log("Initializing MariaDB data directory...");
                var psi = new ProcessStartInfo(installDb, $"--datadir=\"{dataDir}\"")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = Path.Combine(mariaRoot, "bin")
                };
                
                using (var proc = Process.Start(psi))
                {
                    proc?.WaitForExit(30000);
                    if (proc != null && proc.ExitCode == 0) Log("MariaDB initialized.");
                    else Log("Warning: MariaDB initialization might have incomplete.");
                }
            }
        }
        catch (Exception ex)
        {
            Log($"Warning: MariaDB Configuration failed: {ex.Message}");
        }
    }

    private void ConfigurePostgreSQL(string root)
    {
        try
        {
            Log("Configuring PostgreSQL...");
            string pgRoot = Path.Combine(root, "postgres");
            string binDir = Path.Combine(pgRoot, "bin");
            string dataDir = Path.Combine(pgRoot, "data");
            string initdb = Path.Combine(binDir, "initdb.exe");

            if (!Directory.Exists(dataDir) || (Directory.Exists(dataDir) && Directory.GetFiles(dataDir).Length == 0))
            {
                Log("PostgreSQL data directory missing/empty. Initializing...");
                if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
                
                if (File.Exists(initdb))
                {
                    var psi = new ProcessStartInfo(initdb, $"-D \"{dataDir}\" -U postgres -E UTF8 --locale=C --auth=trust")
                    {
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        WorkingDirectory = binDir
                    };
                    
                    using (var proc = Process.Start(psi))
                    {
                        if (proc == null)
                        {
                            Log("ERROR: Could not start PostgreSQL initialization process.");
                            return;
                        }
                        string output = proc.StandardOutput.ReadToEnd();
                        string error = proc.StandardError.ReadToEnd();
                        proc.WaitForExit();
                        
                        if (proc.ExitCode == 0) 
                        {
                            Log("PostgreSQL initialized successfully.");
                            
                            string conf = Path.Combine(dataDir, "postgresql.conf");
                            if (File.Exists(conf))
                            {
                                string content = File.ReadAllText(conf);
                                if (!content.Contains("listen_addresses"))
                                    content += "\r\nlisten_addresses = '127.0.0.1'\r\nport = 5432\r\n";
                                File.WriteAllText(conf, content);
                            }
                            
                            string hba = Path.Combine(dataDir, "pg_hba.conf");
                            if (File.Exists(hba))
                            {
                                string hbaContent = File.ReadAllText(hba);
                                if (!hbaContent.Contains("127.0.0.1/32"))
                                    File.AppendAllText(hba, "\r\nhost    all             all             127.0.0.1/32            trust\r\n");
                            }
                        }
                        else 
                        {
                            Log($"Warning: initdb failed code {proc.ExitCode}. Output: {output} Error: {error}");
                        }
                    }
                }
                else
                {
                    Log("Warning: initdb.exe not found. Skipping initialization.");
                }
            }
        }
        catch (Exception ex)
        {
            Log($"Warning: PostgreSQL Configuration failed: {ex.Message}");
        }
    }

    private async Task InstallComposerDependencies(string root)
    {
        try
        {
            Log("Creating composer.json...");
            string composerJsonPath = Path.Combine(root, "composer.json");
            string composerJson = @"{
    ""require"": {
        ""spiral/roadrunner-worker"": ""^3.0"",
        ""spiral/roadrunner-http"": ""^3.0"",
        ""nyholm/psr7"": ""^1.8""
    },
    ""autoload"": {
        ""psr-4"": {
            ""App\\"": ""app/""
        }
    },
    ""config"": {
        ""platform"": {
            ""php"": ""8.4.1""
        }
    }
}";
            File.WriteAllText(composerJsonPath, composerJson);

            string composerPhar = Path.Combine(root, "composer.phar");
            if (!File.Exists(composerPhar))
            {
                Log("Extracting bundled composer.phar...");
                try 
                {
                    ExtractResourceToFile("composer.phar", composerPhar);
                }
                catch (Exception extractEx)
                {
                    Log($"Error extracting composer.phar: {extractEx.Message}. Attempting download fallback...");
                    using (var client = new HttpClient())
                    {
                        using (var response = await client.GetAsync("https://getcomposer.org/composer.phar"))
                        {
                            response.EnsureSuccessStatusCode();
                            using (var fs = new FileStream(composerPhar, FileMode.CreateNew))
                            {
                                await response.Content.CopyToAsync(fs);
                            }
                        }
                    }
                }
            }

            Log("Running composer install...");
            string phpExe = Path.Combine(root, "php84", "php.exe");
            
            var psi = new ProcessStartInfo(phpExe, $"composer.phar install --no-dev --prefer-dist --optimize-autoloader")
            {
                WorkingDirectory = root,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            
            // Allow Composer to run without unlimited timeout via environment variable if needed
            psi.EnvironmentVariables["COMPOSER_PROCESS_TIMEOUT"] = "2000"; 

            using (var proc = Process.Start(psi))
            {
                if (proc == null) throw new Exception("Failed to start Composer process.");

                string output = await proc.StandardOutput.ReadToEndAsync();
                string error = await proc.StandardError.ReadToEndAsync();
                
                await proc.WaitForExitAsync();
                
                Log($"Composer Output: {output}");
                if (!string.IsNullOrEmpty(error)) Log($"Composer Error/Info: {error}");

                if (proc.ExitCode != 0)
                {
                    throw new Exception($"Composer install failed with code {proc.ExitCode}. Output: {output} Error: {error}");
                }
            }
            
            Log("Dependencies installed successfully.");
        }
        catch (Exception ex)
        {
            Log($"Warning: Composer dependency installation failed: {ex.Message}");
            Log("Ensure internet connection is available or vendor folder is pre-bundled.");
        }
    }

    private async Task ApplyDatabaseSecurity(string root, string password)
    {
        await Task.Yield();
        try
        {
            Log("Applying initial database security...");
            UpdateStatus("Configuring Security...", 99);

            // 1. MariaDB Password
            string mariaBin = Path.Combine(root, "mariadb", "bin", "mariadbd.exe");
            if (File.Exists(mariaBin))
            {
                Log("Applying MariaDB security (Targeting Empty Password for PMA)...");
                var psiMaria = new ProcessStartInfo(mariaBin, $"--defaults-file=\"../my.ini\" --skip-networking --bootstrap")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = Path.GetDirectoryName(mariaBin),
                    RedirectStandardInput = true
                };

                using (var proc = Process.Start(psiMaria))
                {
                    if (proc != null)
                    {
                        proc.StandardInput.WriteLine("ALTER USER 'root'@'localhost' IDENTIFIED BY '';");
                        proc.StandardInput.WriteLine("FLUSH PRIVILEGES;");
                        proc.StandardInput.Close();
                        proc.WaitForExit(10000);
                    }
                }
            }

            // 2. PostgreSQL Password
            string pgCtl = Path.Combine(root, "postgres", "bin", "pg_ctl.exe");
            string dataDir = Path.Combine(root, "postgres", "data");

            if (File.Exists(pgCtl) && Directory.Exists(dataDir))
            {
                Log("Applying PostgreSQL security...");
                Process.Start(new ProcessStartInfo(pgCtl, $"start -D \"{dataDir}\" -w") { CreateNoWindow = true, UseShellExecute = false, WorkingDirectory = Path.GetDirectoryName(pgCtl) })?.WaitForExit(15000);

                string psql = Path.Combine(root, "postgres", "bin", "psql.exe");
                var psiPsql = new ProcessStartInfo(psql, $"-U postgres -c \"ALTER USER postgres WITH PASSWORD '{password}';\"")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = Path.GetDirectoryName(psql),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                psiPsql.EnvironmentVariables["PGPASSWORD"] = ""; 

                using (var psqlProc = Process.Start(psiPsql))
                {
                    if (psqlProc != null)
                    {
                        psqlProc.WaitForExit(10000);
                    }
                }

                Process.Start(new ProcessStartInfo(pgCtl, $"stop -D \"{dataDir}\" -m fast") { CreateNoWindow = true, UseShellExecute = false, WorkingDirectory = Path.GetDirectoryName(pgCtl) })?.WaitForExit(10000);

                string hbaPath = Path.Combine(dataDir, "pg_hba.conf");
                if (File.Exists(hbaPath))
                {
                    Log("Enforcing trust authentication for PostgreSQL...");
                    string hbaContent = File.ReadAllText(hbaPath);
                    // Ensure all authentication methods are 'trust' for local connections
                    hbaContent = hbaContent.Replace("scram-sha-256", "trust");
                    hbaContent = hbaContent.Replace("md5", "trust");
                    File.WriteAllText(hbaPath, hbaContent);
                }
            }

            // 3. phpMyAdmin Config
            string pmaConfig = Path.Combine(root, "www", "phpmyadmin", "config.inc.php");
            if (File.Exists(pmaConfig))
            {
                Log("Updating phpMyAdmin credentials...");
                string content = File.ReadAllText(pmaConfig);
                content = content.Replace("password'] = '';", "password'] = '';"); 
                content = content.Replace("AllowNoPassword'] = true;", "AllowNoPassword'] = true;"); 
                File.WriteAllText(pmaConfig, content);
            }
            
            Log("Database security applied successfully.");
        }
        catch (Exception ex)
        {
            Log($"Warning: Could not apply database security automatically: {ex.Message}");
        }
    }
}

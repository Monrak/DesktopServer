using System;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace DesktopServerSetup
{
    public partial class MainForm : Form
    {
        private string? logFilePath;

        public MainForm(string[] args)
        {
            InitializeComponent();
            
            if (args.Contains("/uninstall"))
            {
                RunUninstall();
                Environment.Exit(0);
                return;
            }

            WireEvents();
            LoadResources();
        }

        public MainForm() : this(Array.Empty<string>()) {}

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
            btnStart.Click += async (s, e) => await StartInstallation();
        }

        private void LoadResources()
        {
            this.Icon = LoadIconFromResource("app_icon.ico");
        }

        private Image? LoadImageFromResource(string resourceName)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                string fullResourceName = $"DesktopServerSetup.Resources.{resourceName}";
                using (Stream? stream = assembly.GetManifestResourceStream(fullResourceName))
                {
                    if (stream != null) return Image.FromStream(stream);
                }
                Log($"Warning: Image resource '{fullResourceName}' not found.");
            }
            catch (Exception ex)
            {
                Log($"Failed to load image {resourceName}: {ex.Message}");
            }
            return null;
        }

        private Icon? LoadIconFromResource(string resourceName)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                string fullResourceName = $"DesktopServerSetup.Resources.{resourceName}";
                using (Stream? stream = assembly.GetManifestResourceStream(fullResourceName))
                {
                    if (stream != null) return new Icon(stream);
                }
                Log($"Warning: Icon resource '{fullResourceName}' not found.");
            }
            catch (Exception ex)
            {
                Log($"Failed to load icon {resourceName}: {ex.Message}");
            }
            return null;
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

            // Write to install.log file
            if (!string.IsNullOrEmpty(logFilePath))
            {
                try
                {
                    File.AppendAllText(logFilePath, logLine + Environment.NewLine);
                }
                catch { /* Ignore logging errors */ }
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
            string baseRoot = txtInstallPath.Text.Trim();
            if (string.IsNullOrEmpty(baseRoot))
            {
                MessageBox.Show("Please select a valid path.");
                return;
            }

            string rootPath = baseRoot;
            string wwwPath = Path.Combine(rootPath, "www");
            logFilePath = Path.Combine(rootPath, "install.log");

            btnStart.Enabled = false;
            btnBrowse.Enabled = false;
            txtInstallPath.ReadOnly = true;

            try
            {
                if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
                
                // Initialize log file
                File.WriteAllText(logFilePath, $"--- Installation Started at {DateTime.Now} ---" + Environment.NewLine);
                Log($"Installation started at {rootPath}");

                UpdateStatus("Preparing directories...", 5);
                if (!Directory.Exists(wwwPath)) Directory.CreateDirectory(wwwPath);

                // Setup Initial Landing Page
                Log("Creating initial landing page...");
                string indexDest = Path.Combine(wwwPath, "index.html");
                if (!File.Exists(indexDest))
                {
                    ExtractResourceToFile("index.html", indexDest);
                }

                // Setup Logo
                string logoDest = Path.Combine(wwwPath, "app_logo.png");
                if (!File.Exists(logoDest))
                {
                    ExtractResourceToFile("app_logo.png", logoDest);
                }


                // Setup Components from Embedded Resources
                await SetupComponentFromResource(rootPath, "Apache 2.4", "apache24_tmp.zip", "apache24", 10, 30);
                await SetupComponentFromResource(rootPath, "PHP 7.4", "php74.zip", "php74", 30, 50);
                await SetupComponentFromResource(rootPath, "MySQL 8.0.27", "mysql80_tmp.zip", "mysql80", 50, 80);
                await SetupComponentFromResource(rootPath, "phpMyAdmin", "pma_tmp.zip", "phpmyadmin", 80, 90);

                UpdateStatus("Finalizing folders...", 95);
                FinalizeFolders(rootPath, wwwPath);

                UpdateStatus("Configuring stack...", 98);
                ConfigureStack(rootPath, wwwPath);

                // Copy Manager from Resource
                Log("Deploying DesktopServerManager...");
                string managerDest = Path.Combine(rootPath, "DesktopServerManager.exe");
                ExtractResourceToFile("DesktopServerManager.exe", managerDest);
                Log("DesktopServerManager deployed successfully.");

                // Register PHP to Path
                string phpPath = Path.Combine(rootPath, "php74");
                RegisterPhpToPath(phpPath);

                UpdateStatus("Completed successfully!", 100);
                Log("SUCCESS: DesktopServerManager is ready.");

                // Create Uninstaller
                string uninstallerPath = Path.Combine(rootPath, "Uninstaller.exe");
                try
                {
                   File.Copy(Application.ExecutablePath, uninstallerPath, true);
                   RegisterUninstaller(rootPath, uninstallerPath);
                   Log("Uninstaller registered successfully.");
                }
                catch (Exception ex)
                {
                    Log($"Warning: Failed to create uninstaller: {ex.Message}");
                }
                
                
                string successMsg = $"Installation Complete!\n\nLocation: {rootPath}\nYour Projects: {wwwPath}\nphpMyAdmin: {wwwPath}\\phpmyadmin\n\nYou can now run DesktopServerManager.exe to manage services.";
                
                // Auto-start logic
                Log("Attempting to launch DesktopServerManager...");
                if (File.Exists(managerDest))
                {
                    try {
                        var startInfo = new ProcessStartInfo(managerDest) { 
                            UseShellExecute = true,
                            WorkingDirectory = rootPath
                        };
                        Process.Start(startInfo);
                        Log("DesktopServerManager launched successfully.");
                        
                        // Close Setup immediately after launch
                        await Task.Delay(500); // Slight delay to ensure log is written
                        Application.Exit();
                        return;
                    } catch (Exception ex) {
                        Log($"CRITICAL: Failed to launch Manager: {ex.Message}");
                        MessageBox.Show(successMsg + "\n\n(Note: DesktopServerManager failed to start automatically, please run it manually.)", "Installed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    Log("CRITICAL: Manager EXE not found at " + managerDest);
                    MessageBox.Show("Installation seemed to succeed, but the Manager executable was not found. Please check your antivirus or installation path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Log($"ERROR: {ex.Message}");
                Log($"Stack Trace: {ex.StackTrace}");
                UpdateStatus("Installation failed.", progressBar.Value);
                MessageBox.Show($"Installation failed: {ex.Message}\n\nCheck install.log for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnStart.Enabled = true;
                btnBrowse.Enabled = true;
                txtInstallPath.ReadOnly = false;
            }
        }

        private async Task SetupComponentFromResource(string root, string name, string resourceFileName, string targetFolder, int startProg, int endProg)
        {
            UpdateStatus($"Setting up {name}...", startProg);
            string targetPath = Path.Combine(root, targetFolder);
            if (Directory.Exists(targetPath) && Directory.GetFileSystemEntries(targetPath).Any())
            {
                Log($"{name} already exists. Skipping.");
                return;
            }

            Log($"Extracting {name} from bundle...");
            string tempZip = Path.Combine(Path.GetTempPath(), resourceFileName);
            
            try
            {
                ExtractResourceToFile(resourceFileName, tempZip);
                
                UpdateStatus($"Extracting {name} files...", (startProg + endProg) / 2);
                string tmpDir = Path.Combine(root, targetFolder + "_tmp");
                if (Directory.Exists(tmpDir)) Directory.Delete(tmpDir, true);
                Directory.CreateDirectory(tmpDir);

                await Task.Run(() => ZipFile.ExtractToDirectory(tempZip, tmpDir));
                Log($"{name} extraction completed.");
            }
            finally
            {
                if (File.Exists(tempZip)) File.Delete(tempZip);
            }
        }

        private void ExtractResourceToFile(string resourceName, string destPath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string fullResourceName = $"DesktopServerSetup.Resources.{resourceName}";
            using (Stream? stream = assembly.GetManifestResourceStream(fullResourceName))
            {
                if (stream == null) throw new Exception($"Resource '{resourceName}' not found in bundle.");
                using (FileStream fs = new FileStream(destPath, FileMode.Create))
                {
                    stream.CopyTo(fs);
                }
            }
        }

        private void FinalizeFolders(string root, string www)
        {
            Log("Finalizing directory structures...");
            
            // Apache
            string apacheTmp = Path.Combine(root, "apache24_tmp");
            string apacheDest = Path.Combine(root, "apache24");
            if (Directory.Exists(apacheTmp))
            {
                string source = Path.Combine(apacheTmp, "Apache24");
                if (Directory.Exists(source)) {
                    Log("Moving Apache (subfolder)...");
                    MoveFolderIfExists(source, apacheDest, true);
                } else {
                    Log("Moving Apache (root)...");
                    MoveFolderIfPossible(apacheTmp, apacheDest);
                }
                DeleteDirectoryIgnoreError(apacheTmp);
            }

            // MySQL
            string mysqlTmp = Path.Combine(root, "mysql80_tmp");
            string mysqlDest = Path.Combine(root, "mysql80");
            if (Directory.Exists(mysqlTmp))
            {
                var dirs = Directory.GetDirectories(mysqlTmp);
                if (dirs.Length > 0) {
                    Log("Moving MySQL (subfolder)...");
                    MoveFolderIfExists(dirs.First(), mysqlDest, true);
                } else {
                    Log("Moving MySQL (root)...");
                    MoveFolderIfPossible(mysqlTmp, mysqlDest);
                }
                DeleteDirectoryIgnoreError(mysqlTmp);
            }

            // PHP
            string phpTmp = Path.Combine(root, "php74_tmp");
            string phpDest = Path.Combine(root, "php74");
            if (Directory.Exists(phpTmp))
            {
                Log("Moving PHP...");
                MoveFolderIfExists(phpTmp, phpDest, false);
                DeleteDirectoryIgnoreError(phpTmp);
            }

            // phpMyAdmin
            string pmaTmp = Path.Combine(root, "phpmyadmin_tmp");
            string pmaDest = Path.Combine(www, "phpmyadmin");
            if (Directory.Exists(pmaTmp))
            {
                var dirs = Directory.GetDirectories(pmaTmp);
                if (dirs.Length > 0) {
                    Log("Moving phpMyAdmin (subfolder)...");
                    MoveFolderIfExists(dirs.First(), pmaDest, true);
                } else {
                    Log("Moving phpMyAdmin (root)...");
                    MoveFolderIfPossible(pmaTmp, pmaDest);
                }
                DeleteDirectoryIgnoreError(pmaTmp);
            }
        }

        private void MoveFolderIfPossible(string source, string dest)
        {
            if (!Directory.Exists(source)) return;
            if (Directory.Exists(dest)) try { Directory.Delete(dest, true); } catch { }
            try { Directory.Move(source, dest); } catch (Exception ex) { Log($"Move failed: {ex.Message}"); }
        }

        private void DeleteDirectoryIgnoreError(string path)
        {
            if (!Directory.Exists(path)) return;
            try { Directory.Delete(path, true); } catch { }
        }

        private void MoveFolderIfExists(string source, string dest, bool deleteTmp)
        {
            if (!Directory.Exists(source)) return;
            try 
            {
                if (Directory.Exists(dest)) 
                {
                    Log($"Removing existing folder: {dest}");
                    Directory.Delete(dest, true);
                }
                
                Log($"Moving: {source} -> {dest}");
                Directory.Move(source, dest);
            }
            catch (Exception ex)
            {
                Log($"Critical Move Failure: {ex.Message}");
                throw; // Rethrow to show error box
            }
        }

        private void ConfigureStack(string root, string www)
        {
            Log("Configuring Apache and PHP...");
            string httpdConf = Path.Combine(root, @"apache24\conf\httpd.conf");
            string phpPath = Path.Combine(root, "php74");
            if (File.Exists(httpdConf) && Directory.Exists(phpPath))
            {
                string content = File.ReadAllText(httpdConf);
                string rootUrl = root.Replace("\\", "/");
                string wwwUrl = www.Replace("\\", "/");
                string phpPathUrl = phpPath.Replace("\\", "/");

                // Robust Replacement using Regex
                // 1. Define SRVROOT
                content = System.Text.RegularExpressions.Regex.Replace(content, 
                    @"^Define\s+SRVROOT\s+"".*""", 
                    $"Define SRVROOT \"{rootUrl}/apache24\"", 
                    System.Text.RegularExpressions.RegexOptions.Multiline);

                // 2. ServerRoot (sometimes used instead of SRVROOT)
                content = System.Text.RegularExpressions.Regex.Replace(content, 
                    @"^ServerRoot\s+"".*""", 
                    $"ServerRoot \"{rootUrl}/apache24\"", 
                    System.Text.RegularExpressions.RegexOptions.Multiline);

                // 3. DocumentRoot
                content = System.Text.RegularExpressions.Regex.Replace(content, 
                    @"^DocumentRoot\s+"".*""", 
                    $"DocumentRoot \"{wwwUrl}\"", 
                    System.Text.RegularExpressions.RegexOptions.Multiline);

                // 4. Directory index for DocumentRoot
                // We find the tag that looks like <Directory "..."> and replace its path
                // Note: This is a bit tricky, so we target the standard ${SRVROOT}/htdocs or any path
                content = System.Text.RegularExpressions.Regex.Replace(content, 
                    @"<Directory\s+"".*?"">", 
                    (m) => m.Value.Contains("cgi-bin") ? m.Value : $"<Directory \"{wwwUrl}\">", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // --- NEW: Auto-Config Enhancements ---
                // 1. Enable .htaccess (AllowOverride All for the document root)
                // Note: We do this after Replacing the Directory tag so we are reasonably sure we are in the right context
                content = content.Replace("AllowOverride None", "AllowOverride All"); 

                // 2. Enable mod_rewrite
                content = content.Replace("#LoadModule rewrite_module modules/mod_rewrite.so", "LoadModule rewrite_module modules/mod_rewrite.so");

                // 3. Set PHP as priority index
                content = content.Replace("DirectoryIndex index.html", "DirectoryIndex index.php index.html index.htm");

                // 4. Security Headers
                if (!content.Contains("ServerTokens Prod"))
                {
                    content += "\n# Security Headers\nServerTokens Prod\nServerSignature Off\n";
                }
                // -------------------------------------

                // Add PHP Integration if not present
                if (!content.Contains("php7_module"))
                {
                    string phpMod = $"\n# PHP 7.4 Integration\nPHPIniDir \"{phpPathUrl}\"\nLoadModule php7_module \"{phpPathUrl}/php7apache2_4.dll\"\nAddType application/x-httpd-php .php\n";
                    content += phpMod;
                }
                
                File.WriteAllText(httpdConf, content);
            }

            string phpIni = Path.Combine(phpPath, "php.ini");
            if (!File.Exists(phpIni))
            {
                string phpIniDev = Path.Combine(phpPath, "php.ini-development");
                if (File.Exists(phpIniDev))
                {
                    File.Copy(phpIniDev, phpIni);
                    string ini = File.ReadAllText(phpIni);
                    
                    // --- NEW: PHP Optimization ---
                    // 1. Extensions
                    ini = ini.Replace(";extension_dir = \"ext\"", "extension_dir = \"ext\"");
                    ini = ini.Replace(";extension=mysqli", "extension=mysqli");
                    ini = ini.Replace(";extension=pdo_mysql", "extension=pdo_mysql");
                    ini = ini.Replace(";extension=curl", "extension=curl");
                    ini = ini.Replace(";extension=gd2", "extension=gd2");
                    ini = ini.Replace(";extension=mbstring", "extension=mbstring");
                    ini = ini.Replace(";extension=exif", "extension=exif");
                    ini = ini.Replace(";extension=openssl", "extension=openssl");
                    ini = ini.Replace(";extension=soap", "extension=soap");
                    ini = ini.Replace(";extension=zip", "extension=zip");
                    
                    // 2. Resource Limits
                    ini = ini.Replace("memory_limit = 128M", "memory_limit = 512M");
                    ini = ini.Replace("upload_max_filesize = 2M", "upload_max_filesize = 2048M");
                    ini = ini.Replace("post_max_size = 8M", "post_max_size = 2048M");
                    ini = ini.Replace("max_execution_time = 30", "max_execution_time = 600");
                    ini = ini.Replace("max_input_time = 60", "max_input_time = 600");

                    // 3. Timezone & Errors
                    ini = ini.Replace(";date.timezone =", "date.timezone = Asia/Bangkok");
                    ini = ini.Replace("display_errors = Off", "display_errors = On");
                    ini = ini.Replace("display_startup_errors = Off", "display_startup_errors = On");
                    // -----------------------------

                    File.WriteAllText(phpIni, ini);
                }
            }

            File.WriteAllText(Path.Combine(root, @"mysql80\my.ini"), $"[mysqld]\nbasedir=\"{Path.Combine(root, "mysql80").Replace("\\", "/")}\"\ndatadir=\"{Path.Combine(root, @"mysql80\data").Replace("\\", "/")}\"\nport=3306\n");

            // --- NEW: phpMyAdmin Configuration ---
            Log("Configuring phpMyAdmin...");
            string pmaPath = Path.Combine(www, "phpmyadmin");
            string pmaConfig = Path.Combine(pmaPath, "config.inc.php");
            if (Directory.Exists(pmaPath))
            {
                string pmaConfigContent = "<?php\n" +
                    "$cfg['blowfish_secret'] = 'desktopserver_secret_key_32_chars_long';\n" +
                    "$i = 0;\n" +
                    "$i++;\n" +
                    "$cfg['Servers'][$i]['auth_type'] = 'config';\n" +
                    "$cfg['Servers'][$i]['host'] = '127.0.0.1';\n" +
                    "$cfg['Servers'][$i]['user'] = 'root';\n" +
                    "$cfg['Servers'][$i]['password'] = '';\n" +
                    "$cfg['Servers'][$i]['compress'] = false;\n" +
                    "$cfg['Servers'][$i]['AllowNoPassword'] = true;\n" +
                    "$cfg['Servers'][$i]['DisableIS'] = true;\n" +
                    "$cfg['UploadDir'] = '';\n" +
                    "$cfg['SaveDir'] = '';\n" +
                    "?>";
                File.WriteAllText(pmaConfig, pmaConfigContent);
                Log("phpMyAdmin config.inc.php created with AllowNoPassword=true.");
            }
        }

        private void RegisterUninstaller(string installLocation, string uninstallerPath)
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\DesktopServer"))
                {
                    key.SetValue("DisplayName", "Monrak Desktop Server");
                    key.SetValue("DisplayVersion", "1.0.0");
                    key.SetValue("Publisher", "Monrak Net Co., Ltd.");
                    key.SetValue("UninstallString", $"\"{uninstallerPath}\" /uninstall");
                    key.SetValue("InstallLocation", installLocation);
                    key.SetValue("DisplayIcon", uninstallerPath);
                    key.SetValue("NoModify", 1);
                    key.SetValue("NoRepair", 1);
                }
            }
            catch (Exception ex)
            {
                Log($"Failed to register uninstaller: {ex.Message}");
            }
        }

        private void RunUninstall()
        {
            try
            {
                string self = Application.ExecutablePath;
                string root = Path.GetDirectoryName(self) ?? "";
                
                if (MessageBox.Show("Are you sure you want to uninstall Monrak Desktop Server?\n\nThis will stop all running services and remove the application.", "Uninstall", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }

                // 1. Kill Processes
                string[] procs = { "DesktopServerManager", "httpd", "mysqld", "php-cgi" };
                foreach (var pName in procs)
                {
                    foreach (var p in Process.GetProcessesByName(pName))
                    {
                        try { p.Kill(); p.WaitForExit(1000); } catch {}
                    }
                }

                // 2. Remove Registry
                try
                {
                    Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\DesktopServer", false);
                }
                catch {}

                // 3. Remove Files (Attempt)
                if (!string.IsNullOrEmpty(root) && Directory.Exists(root))
                {
                    // Create a temporary cleanup script
                    string tempDir = Path.GetTempPath();
                    string batchFile = Path.Combine(tempDir, "stackserver_cleanup.bat");
                    
                    File.WriteAllText(batchFile, $@"
@echo off
timeout /t 2 /nobreak >nul
rmdir /s /q ""{root}""
del ""%~f0""
");
                    
                    Process.Start(new ProcessStartInfo(batchFile) 
                    { 
                        CreateNoWindow = true, 
                        UseShellExecute = true, 
                        WindowStyle = ProcessWindowStyle.Hidden 
                    });
                }
                
                MessageBox.Show("Uninstallation complete.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Uninstall Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RegisterPhpToPath(string phpPath)
        {
            try
            {
                Log("Registering PHP to User Environment Path...");
                const string pathKey = "Path";
                var scope = EnvironmentVariableTarget.User;
                string currentPath = Environment.GetEnvironmentVariable(pathKey, scope) ?? "";

                if (!currentPath.Split(';').Any(p => p.Trim().Equals(phpPath, StringComparison.OrdinalIgnoreCase)))
                {
                    string newPath = currentPath.TrimEnd(';') + ";" + phpPath;
                    Environment.SetEnvironmentVariable(pathKey, newPath, scope);
                    Log("PHP successfully added to Path.");
                }
                else
                {
                    Log("PHP already exists in Path.");
                }
            }
            catch (Exception ex)
            {
                Log($"Failed to register PHP to Path: {ex.Message}");
            }
        }
    }
}

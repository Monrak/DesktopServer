using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;

namespace DesktopServerSetupPro
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
            btnBrowse.Click += (s, e) =>
            {
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
                string fullResourceName = $"DesktopServerSetupPro.Resources.{resourceName}";
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
                string fullResourceName = $"DesktopServerSetupPro.Resources.{resourceName}";
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

                // Setup Favicon
                string faviconDest = Path.Combine(wwwPath, "favicon.ico");
                if (!File.Exists(faviconDest))
                {
                    Log("Deploying favicon...");
                    ExtractResourceToFile("app_icon.ico", faviconDest);
                }


                // Setup Components from Embedded Resources
                await SetupComponentFromResource(rootPath, "Apache 2.4", "apache24_tmp.zip", "apache24", 10, 30);
                await SetupComponentFromResource(rootPath, "PHP 7.4", "php74.zip", "php74", 30, 40);
                await SetupComponentFromResource(rootPath, "PHP 5.6", "php56.zip", "php56", 40, 50);
                await SetupComponentFromResource(rootPath, "PHP 8.2", "php82.zip", "php82", 50, 60);
                await SetupComponentFromResource(rootPath, "MySQL 8.0.27", "mysql80_tmp.zip", "mysql80", 60, 75);
                await SetupComponentFromResource(rootPath, "phpMyAdmin (Main)", "pma_tmp.zip", "phpmyadmin", 70, 80);
                await SetupComponentFromResource(rootPath, "phpMyAdmin (5.6)", "pma56_tmp.zip", "phpmyadmin56", 80, 90);

                UpdateStatus("Finalizing folders...", 95);
                FinalizeFolders(rootPath, wwwPath);

                UpdateStatus("Configuring stack...", 98);
                ConfigureStack(rootPath, wwwPath);

                // Copy Manager from Resource
                Log("Deploying DesktopServerManagerPro...");
                string managerDest = Path.Combine(rootPath, "DesktopServerManagerPro.exe");
                ExtractResourceToFile("DesktopServerManagerPro.exe", managerDest);
                Log("DesktopServerManagerPro deployed successfully.");

                // Removed RegisterPhpToPath call as we now use isolated environments in Manager
                // string phpPath = Path.Combine(rootPath, "php74");
                // RegisterPhpToPath(phpPath);

                // NEW: Clean up ANY existing PHP paths from User Path to avoid conflicts
                CleanPhpFromPath();

                UpdateStatus("Completed successfully!", 100);
                UpdateStatus("Completed successfully!", 100);
                Log("SUCCESS: DesktopServer Pro is ready.");

                UpdateStatus("Registering Uninstaller...", 98);
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

                // Create Start Menu Shortcut
                UpdateStatus("Creating Start Menu Shortcut...", 99);
                CreateShortcut(Path.Combine(rootPath, "DesktopServerManagerPro.exe"), "Monrak Manager Pro!", "Launch Monrak Desktop Server Pro!");

                UpdateStatus("Completed successfully!", 100);
                Log("SUCCESS: Monrak Desktop Server Pro! is ready.");

                // --- VC++ Redist Optional Install ---
                var vcResult = MessageBox.Show("Do you want to install Microsoft Visual C++ Redistributable (x64)?\n\n[IMPORTANT] This is MANDATORY for PHP 5.6 to work. Without this, PHP 5.6 will not start.",
                    "Required for PHP 5.6", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (vcResult == DialogResult.Yes)
                {
                    Log("User chose to install VC++ Redistributable (Mandatory for PHP 5.6)...");
                    try
                    {
                        string vcredistPath = Path.Combine(Path.GetTempPath(), "vcredist_x64.exe");
                        ExtractResourceToFile("vcredist_x64.exe", vcredistPath);
                        Log("Running vcredist_x64.exe...");
                        var vcProcess = Process.Start(new ProcessStartInfo(vcredistPath) { UseShellExecute = true });
                        vcProcess?.WaitForExit();
                        Log("VC++ Redistributable installation finished.");
                    }
                    catch (Exception ex)
                    {
                        Log($"Failed to install VC++ Redist: {ex.Message}");
                        MessageBox.Show("Failed to launch VC++ Redist installer. Please install it manually so PHP 5.6 can work.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    Log("User skipped VC++ Redistributable installation.");
                    MessageBox.Show("Warning: PHP 5.6 will NOT work because you skipped the VC++ Redistributable installation.",
                        "Compatibility Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // -------------------------------------

                string successMsg = $"Installation Complete!\n\nLocation: {rootPath}\nYour Projects: {wwwPath}\nphpMyAdmin: {wwwPath}\\phpmyadmin\n\nYou can now run DesktopServerManagerPro.exe to manage services.";
                
                // Auto-start logic
                Log("Attempting to launch DesktopServerManagerPro...");
                if (File.Exists(managerDest))
                {
                    try {
                        var startInfo = new ProcessStartInfo(managerDest) { 
                            UseShellExecute = true,
                            WorkingDirectory = rootPath
                        };
                        Process.Start(startInfo);
                        Log("DesktopServerManagerPro launched successfully.");
                        
                        // Close Setup immediately after launch
                        await Task.Delay(500); // Slight delay to ensure log is written
                        Application.Exit();
                        return;
                    } catch (Exception ex) {
                        Log($"CRITICAL: Failed to launch Manager: {ex.Message}");
                        MessageBox.Show(successMsg + "\n\n(Note: DesktopServerManagerPro failed to start automatically, please run it manually.)", "Installed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            // We want to overwrite to ensure fresh files, so we don't skip anymore
            if (Directory.Exists(targetPath))
            {
                Log($"{name} exists. Preparing for fresh install...");
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
        // Try logical name first
        string fullResourceName = $"DesktopServerSetupPro.Resources.{resourceName}";
        
        using (Stream? stream = assembly.GetManifestResourceStream(fullResourceName))
        {
            if (stream == null) 
            {
                // Fallback: Try finding it by suffix
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

                Log($"Error: Resource '{resourceName}' not found. Available: {string.Join(", ", allResources)}");
                throw new Exception($"Resource '{resourceName}' not found in bundle.");
            }

            using (FileStream fs = new FileStream(destPath, FileMode.Create))
            {
                stream.CopyTo(fs);
            }
        }
    }

    private void CreateShortcut(string targetPath, string shortcutName, string description)
    {
        try
        {
            Log($"Creating Start Menu shortcut: {shortcutName}...");
            string startMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            string appStartMenuPath = Path.Combine(startMenuPath, "Monrak Net");
            
            if (!Directory.Exists(appStartMenuPath))
            {
                Directory.CreateDirectory(appStartMenuPath);
            }

            string shortcutLocation = Path.Combine(appStartMenuPath, shortcutName + ".lnk");
            
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

        private void FinalizeFolders(string root, string www)
        {
            Log("Finalizing directory structures...");

            // Apache
            string apacheTmp = Path.Combine(root, "apache24_tmp");
            string apacheDest = Path.Combine(root, "apache24");
            if (Directory.Exists(apacheTmp))
            {
                string source = Path.Combine(apacheTmp, "Apache24");
                if (Directory.Exists(source))
                {
                    Log("Moving Apache (subfolder)...");
                    MoveFolderIfExists(source, apacheDest, true);
                }
                else
                {
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
                if (dirs.Length > 0)
                {
                    Log("Moving MySQL (subfolder)...");
                    MoveFolderIfExists(dirs.First(), mysqlDest, true);
                }
                else
                {
                    Log("Moving MySQL (root)...");
                    MoveFolderIfPossible(mysqlTmp, mysqlDest);
                }
                DeleteDirectoryIgnoreError(mysqlTmp);
            }

            // PHP 7.4
            string php74Tmp = Path.Combine(root, "php74_tmp");
            string php74Dest = Path.Combine(root, "php74");
            if (Directory.Exists(php74Tmp))
            {
                Log("Moving PHP 7.4...");
                MoveFolderIfExists(php74Tmp, php74Dest, false);
                DeleteDirectoryIgnoreError(php74Tmp);
            }

            // PHP 5.6
            string php56Tmp = Path.Combine(root, "php56_tmp");
            string php56Dest = Path.Combine(root, "php56");
            if (Directory.Exists(php56Tmp))
            {
                Log("Moving PHP 5.6...");
                MoveFolderIfExists(php56Tmp, php56Dest, false);
                DeleteDirectoryIgnoreError(php56Tmp);
            }

            // PHP 8.2
            string php82Tmp = Path.Combine(root, "php82_tmp");
            string php82Dest = Path.Combine(root, "php82");
            if (Directory.Exists(php82Tmp))
            {
                Log("Moving PHP 8.2...");
                MoveFolderIfExists(php82Tmp, php82Dest, false);
                DeleteDirectoryIgnoreError(php82Tmp);
            }

            // phpMyAdmin
            string pmaTmp = Path.Combine(root, "phpmyadmin_tmp");
            string pmaDest = Path.Combine(www, "phpmyadmin");
            if (Directory.Exists(pmaTmp))
            {
                var dirs = Directory.GetDirectories(pmaTmp);
                if (dirs.Length > 0)
                {
                    Log("Moving phpMyAdmin (subfolder)...");
                    MoveFolderIfExists(dirs.First(), pmaDest, true);
                }
                else
                {
                    Log("Moving phpMyAdmin (root)...");
                    MoveFolderIfPossible(pmaTmp, pmaDest);
                }
                DeleteDirectoryIgnoreError(pmaTmp);
            }

            // phpMyAdmin 5.6
            string pma56Tmp = Path.Combine(root, "phpmyadmin56_tmp");
            string pma56Dest = Path.Combine(www, "phpmyadmin56");
            if (Directory.Exists(pma56Tmp))
            {
                var dirs = Directory.GetDirectories(pma56Tmp);
                if (dirs.Length > 0)
                {
                    Log("Moving phpMyAdmin 5.6 (subfolder)...");
                    MoveFolderIfExists(dirs.First(), pma56Dest, true);
                }
                else
                {
                    Log("Moving phpMyAdmin 5.6 (root)...");
                    MoveFolderIfPossible(pma56Tmp, pma56Dest);
                }
                DeleteDirectoryIgnoreError(pma56Tmp);
            }

            // CRITICAL: Copy PHP Core DLLs to Apache Bin for legacy compatibility (PHP 5.x)
            // This ensures extensions like curl, openssl, and mysqli always find their dependencies.
            try
            {
                string apacheBin = Path.Combine(root, @"apache24\bin");
                if (Directory.Exists(apacheBin))
                {
                    var php56Path = Path.Combine(root, "php56");
                    if (Directory.Exists(php56Path))
                    {
                        Log("Copying PHP 5.6 Core DLLs to Apache bin...");
                        string[] legacyDlls = { "libeay32.dll", "ssleay32.dll", "libmysql.dll", "php5ts.dll" };
                        foreach (var dll in legacyDlls)
                        {
                            string src = Path.Combine(php56Path, dll);
                            if (File.Exists(src)) File.Copy(src, Path.Combine(apacheBin, dll), true);
                        }
                    }
                }
            }
            catch (Exception ex) { Log($"DLL Copy Warning: {ex.Message}"); }
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


            // Loop through ALL PHP folders to apply optimization (php56, php82, etc.)
            var phpDirs = Directory.GetDirectories(root, "php*");
            foreach (var pDir in phpDirs)
            {
                string iniPath = Path.Combine(pDir, "php.ini");
                // If php.ini doesn't exist, try copying from development template
                if (!File.Exists(iniPath))
                {
                    string devIni = Path.Combine(pDir, "php.ini-development");
                    if (File.Exists(devIni)) File.Copy(devIni, iniPath);
                }

                if (File.Exists(iniPath))
                {
                    string ini = File.ReadAllText(iniPath);
                    // Check if already optimized to avoid double processing or just overwrite

                    // 1. Extensions - Robust Regex approach
                    // Enable extension_dir with ABSOLUTE path
                    string absoluteExtDir = Path.Combine(pDir, "ext").Replace("\\", "/");
                    ini = System.Text.RegularExpressions.Regex.Replace(ini,
                        @"^;\s*extension_dir\s*=\s*""ext""", $"extension_dir = \"{absoluteExtDir}\"",
                        System.Text.RegularExpressions.RegexOptions.Multiline);
                    ini = System.Text.RegularExpressions.Regex.Replace(ini,
                        @"^extension_dir\s*=\s*""ext""", $"extension_dir = \"{absoluteExtDir}\"",
                        System.Text.RegularExpressions.RegexOptions.Multiline);

                    // List of extensions to enable (handling both legacy php_*.dll and modern names)
                    string[] extensionsToEnable = {
                         "mysqli", "php_mysqli.dll",
                         "pdo_mysql", "php_pdo_mysql.dll",
                         "curl", "php_curl.dll",
                         "gd2", "php_gd2.dll", "gd",
                         "mbstring", "php_mbstring.dll",
                         "exif", "php_exif.dll",
                         "openssl", "php_openssl.dll",
                         "soap", "php_soap.dll",
                         "zip", "php_zip.dll"
                     };

                    foreach (var ext in extensionsToEnable)
                    {
                        ini = System.Text.RegularExpressions.Regex.Replace(ini,
                            $@"^;\s*extension\s*=\s*{System.Text.RegularExpressions.Regex.Escape(ext)}",
                            $"extension={ext}",
                            System.Text.RegularExpressions.RegexOptions.Multiline);
                    }

                    // 2. Resource Limits
                    ini = ini.Replace("memory_limit = 128M", "memory_limit = 512M");
                    ini = ini.Replace("upload_max_filesize = 2M", "upload_max_filesize = 2048M");
                    ini = ini.Replace("post_max_size = 8M", "post_max_size = 2048M");
                    ini = ini.Replace("max_execution_time = 30", "max_execution_time = 600");
                    // 3. Timezone & Errors
                    ini = System.Text.RegularExpressions.Regex.Replace(ini,
                        @"^;\s*date\.timezone\s*=", "date.timezone = Asia/Bangkok",
                        System.Text.RegularExpressions.RegexOptions.Multiline);

                    File.WriteAllText(iniPath, ini);
                    Log($"Configured php.ini for {Path.GetFileName(pDir)} (Robust Mode)");
                }
            }

            File.WriteAllText(Path.Combine(root, @"mysql80\my.ini"),
                $"[mysqld]\n" +
                $"basedir=\"{Path.Combine(root, "mysql80").Replace("\\", "/")}\"\n" +
                $"datadir=\"{Path.Combine(root, @"mysql80\data").Replace("\\", "/")}\"\n" +
                $"port=3306\n" +
                $"# Compatibility for PHP 5.6\n" +
                $"default_authentication_plugin=mysql_native_password\n" +
                $"character-set-server=utf8\n" +
                $"collation-server=utf8_general_ci\n" +
                $"skip-character-set-client-handshake\n");

            // --- NEW: phpMyAdmin Configuration ---
            Log("Configuring phpMyAdmin...");
            string pmaPath = Path.Combine(www, "phpmyadmin");
            string pmaConfig = Path.Combine(pmaPath, "config.inc.php");
            if (Directory.Exists(pmaPath))
            {
                string pmaConfigContent = "<?php\n" +
                    "$cfg['blowfish_secret'] = 'monraknet_secret_key_32_chars_long';\n" +
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

            // Config for pma56
            string pma56Path = Path.Combine(www, "phpmyadmin56");
            string pma56Config = Path.Combine(pma56Path, "config.inc.php");
            if (Directory.Exists(pma56Path))
            {
                // Re-use same content
                string pmaConfigContent = "<?php\n" +
                   "$cfg['blowfish_secret'] = 'monraknet_secret_key_32_chars_long';\n" +
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
                File.WriteAllText(pma56Config, pmaConfigContent);
                Log("phpMyAdmin 5.6 config.inc.php created.");
            }
        }

        // Removed RegisterPhpToPath method entirely to keep system clean.

        private void RegisterUninstaller(string installLocation, string uninstallerPath)
        {
            try
            {
                // Registry Key: StackServerPro
                using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\DesktopServerPro"))
                {
                    key.SetValue("DisplayName", "Monrak Desktop Server Pro");
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
                
                if (MessageBox.Show("Are you sure you want to uninstall DesktopServer Pro?\n\nThis will stop all running services and remove the application.", "Uninstall", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }

            // 1. Kill Processes
            string[] procs = { "DesktopServerManagerPro", "httpd", "mysqld", "php-cgi" };
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
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\DesktopServerPro", false);
            }
            catch {}

            // 3. Remove Shortcut
            try
            {
                string startMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
                string lnkPath = Path.Combine(startMenuPath, "Monrak Net", "Monrak Manager Pro!.lnk");
                if (File.Exists(lnkPath)) File.Delete(lnkPath);
                
                // Cleanup empty directory if no other versions exist
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
                    // Create a temporary cleanup script
                    string tempDir = Path.GetTempPath();
                    string batchFile = Path.Combine(tempDir, "desktopserver_cleanup.bat");
                    
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

        private void CleanPhpFromPath()
        {
            try
            {
                Log("Checking and cleaning PHP from Windows Path...");
                const string pathKey = "Path";
                var scope = EnvironmentVariableTarget.User;
                string currentPath = Environment.GetEnvironmentVariable(pathKey, scope) ?? "";

                var paths = currentPath.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
                int removedCount = 0;

                // Remove any path containing "php" (case-insensitive) that looks like a PHP folder
                for (int i = paths.Count - 1; i >= 0; i--)
                {
                    string p = paths[i];
                    if (p.ToLower().Contains("php") && (p.ToLower().Contains("bin") || File.Exists(Path.Combine(p, "php.exe"))))
                    {
                        Log($"Removing conflicting Path: {p}");
                        paths.RemoveAt(i);
                        removedCount++;
                    }
                }

                if (removedCount > 0)
                {
                    string newPath = string.Join(";", paths);
                    Environment.SetEnvironmentVariable(pathKey, newPath, scope);
                    Log($"Cleanup complete. Removed {removedCount} PHP path(s).");
                }
                else
                {
                    Log("No conflicting PHP paths found.");
                }
            }
            catch (Exception ex)
            {
                Log($"Warning: Failed to clean Path: {ex.Message}");
            }
        }
    }
}

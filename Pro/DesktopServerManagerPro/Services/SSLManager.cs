using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DesktopServerManagerPro.Services
{
    public class SSLManager
    {
        private readonly string _apachePath;
        private readonly string _opensslExe;
        private readonly string _confPath;
        private readonly string _certPath;

        public SSLManager(string apachePath)
        {
            _apachePath = apachePath;
            _opensslExe = Path.Combine(_apachePath, "bin", "openssl.exe");
            _confPath = Path.Combine(_apachePath, "conf");
            _certPath = Path.Combine(_apachePath, "conf", "ssl");
        }

        public async Task<bool> IsSSLEnabled()
        {
            string httpdConf = Path.Combine(_confPath, "httpd.conf");
            if (!File.Exists(httpdConf)) return false;

            string content = await File.ReadAllTextAsync(httpdConf);
            return content.Contains("Include conf/extra/httpd-ssl.conf") && !content.Contains("#Include conf/extra/httpd-ssl.conf");
        }

        public async Task<bool> SetupSSL(string domain = "localhost")
        {
            try
            {
                if (!Directory.Exists(_certPath)) Directory.CreateDirectory(_certPath);

                string keyPath = Path.Combine(_certPath, "server.key");
                string crtPath = Path.Combine(_certPath, "server.crt");

                if (!File.Exists(keyPath) || !File.Exists(crtPath))
                {
                    bool generated = await GenerateSelfSignedCertificate(domain, keyPath, crtPath);
                    if (!generated) return false;
                }

                await ConfigureApacheSSL(keyPath, crtPath, domain);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SSL Setup Error: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> GenerateSelfSignedCertificate(string domain, string keyPath, string crtPath)
        {
            if (!File.Exists(_opensslExe)) return false;

            // Modern browsers (Chrome/Edge) require Subject Alternative Name (SAN)
            string configPath = Path.Combine(_certPath, "openssl_temp.cnf");
            string configContent = $@"
[req]
distinguished_name = req_distinguished_name
x509_extensions = v3_req
prompt = no
[req_distinguished_name]
CN = {domain}
[v3_req]
subjectAltName = @alt_names
[alt_names]
DNS.1 = {domain}
DNS.2 = localhost
IP.1 = 127.0.0.1
";
            await File.WriteAllTextAsync(configPath, configContent);

            string args = $"req -x509 -nodes -days 3650 -newkey rsa:2048 -keyout \"{keyPath}\" -out \"{crtPath}\" -config \"{configPath}\" -extensions v3_req";

            ProcessStartInfo psi = new ProcessStartInfo(_opensslExe, args)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = Path.GetDirectoryName(_opensslExe)
            };

            using (Process? proc = Process.Start(psi))
            {
                if (proc == null) return false;
                await proc.WaitForExitAsync();
                
                // Cleanup temp config
                try { if (File.Exists(configPath)) File.Delete(configPath); } catch { }
                
                return proc.ExitCode == 0;
            }
        }

        private async Task ConfigureApacheSSL(string keyPath, string crtPath, string domain)
        {
            string httpdConf = Path.Combine(_confPath, "httpd.conf");
            string sslConf = Path.Combine(_confPath, "extra", "httpd-ssl.conf");

            if (!File.Exists(httpdConf)) return;

            string content = await File.ReadAllTextAsync(httpdConf);

            // Enable modules
            content = content.Replace("#LoadModule ssl_module modules/mod_ssl.so", "LoadModule ssl_module modules/mod_ssl.so");
            content = content.Replace("#LoadModule socache_shmcb_module modules/mod_socache_shmcb.so", "LoadModule socache_shmcb_module modules/mod_socache_shmcb.so");
            
            // Enable Include
            if (!content.Contains("Include conf/extra/httpd-ssl.conf"))
            {
                content += "\nInclude conf/extra/httpd-ssl.conf\n";
            }
            else
            {
                content = content.Replace("#Include conf/extra/httpd-ssl.conf", "Include conf/extra/httpd-ssl.conf");
            }

            await File.WriteAllTextAsync(httpdConf, content);

            // Configure httpd-ssl.conf
            if (File.Exists(sslConf))
            {
                string sContent = await File.ReadAllTextAsync(sslConf);
                string apacheRoot = _apachePath.Replace("\\", "/");
                string keyUrl = keyPath.Replace("\\", "/");
                string crtUrl = crtPath.Replace("\\", "/");
                string wwwUrl = Path.Combine(Path.GetDirectoryName(_apachePath) ?? "", "www").Replace("\\", "/");

                // Update common paths
                sContent = System.Text.RegularExpressions.Regex.Replace(sContent, @"SSLCertificateFile\s+"".*""", $"SSLCertificateFile \"{crtUrl}\"");
                sContent = System.Text.RegularExpressions.Regex.Replace(sContent, @"SSLCertificateKeyFile\s+"".*""", $"SSLCertificateKeyFile \"{keyUrl}\"");
                
                // Ensure DocumentRoot and Directory permissions are correct for SSL
                sContent = System.Text.RegularExpressions.Regex.Replace(sContent, @"DocumentRoot\s+"".*""", $"DocumentRoot \"{wwwUrl}\"");
                
                // Add/Update Directory block for SSL
                if (!sContent.Contains($"<Directory \"{wwwUrl}\">"))
                {
                    string dirBlock = $"\n<Directory \"{wwwUrl}\">\n    Options Indexes FollowSymLinks\n    AllowOverride All\n    Require all granted\n</Directory>\n";
                    sContent = sContent.Replace("</VirtualHost>", dirBlock + "</VirtualHost>");
                }

                // Ensure ServerName is correct
                sContent = System.Text.RegularExpressions.Regex.Replace(sContent, @"ServerName\s+.*:443", $"ServerName {domain}:443");

                await File.WriteAllTextAsync(sslConf, sContent);
            }
        }

        public async Task<bool> DisableSSL()
        {
            string httpdConf = Path.Combine(_confPath, "httpd.conf");
            if (!File.Exists(httpdConf)) return false;

            string content = await File.ReadAllTextAsync(httpdConf);
            content = content.Replace("Include conf/extra/httpd-ssl.conf", "#Include conf/extra/httpd-ssl.conf");
            
            await File.WriteAllTextAsync(httpdConf, content);
            return true;
        }

        public async Task<bool> TrustCertificate()
        {
            string crtPath = Path.Combine(_certPath, "server.crt");
            if (!File.Exists(crtPath)) return false;

            // Use certutil to add to Trusted Root
            ProcessStartInfo psi = new ProcessStartInfo("certutil", $"-addstore -f \"Root\" \"{crtPath}\"")
            {
                Verb = "runas", // Require admin
                CreateNoWindow = true,
                UseShellExecute = true
            };

            try
            {
                using (Process? proc = Process.Start(psi))
                {
                    if (proc == null) return false;
                    await proc.WaitForExitAsync();
                    return proc.ExitCode == 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}

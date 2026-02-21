using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DesktopServerManagerGo.Services
{
    public class RoadRunnerSSLManager
    {
        private readonly string _rootPath;
        private readonly string _rrYamlPath;
        private readonly string _certPath;

        public RoadRunnerSSLManager(string rootPath)
        {
            _rootPath = rootPath;
            _rrYamlPath = Path.Combine(_rootPath, ".rr.yaml");
            _certPath = Path.Combine(_rootPath, "ssl");
        }

        private string? GetOpenSSLExePath()
        {
            // Search order:
            // 1. Root directory (where php.exe often lives in Go edition)
            // 2. bin directory
            // 3. php directory
            // 4. System PATH

            string[] searchPaths = {
                Path.Combine(_rootPath, "openssl"),
                Path.Combine(_rootPath, "php84")
            };

            foreach (var path in searchPaths)
            {
                if (!Directory.Exists(path)) continue;
                string fullPath = Path.Combine(path, "openssl.exe");
                if (File.Exists(fullPath)) return fullPath;

                // Search one level deep (typical for PHP/Apache structures)
                try
                {
                    foreach (var sub in Directory.GetDirectories(path))
                    {
                        string subPath = Path.Combine(sub, "openssl.exe");
                        if (File.Exists(subPath)) return subPath;
                        
                        // Check bin folder inside sub (common in some bundles)
                        string subBinPath = Path.Combine(sub, "bin", "openssl.exe");
                        if (File.Exists(subBinPath)) return subBinPath;
                    }
                }
                catch { }
            }

            // Check system PATH
            var values = Environment.GetEnvironmentVariable("PATH");
            if (values != null)
            {
                foreach (var path in values.Split(Path.PathSeparator))
                {
                    var fullPath = Path.Combine(path, "openssl.exe");
                    if (File.Exists(fullPath)) return fullPath;
                }
            }

            return null;
        }

        public async Task<bool> IsSSLEnabled()
        {
            if (!File.Exists(_rrYamlPath)) return false;
            string content = await File.ReadAllTextAsync(_rrYamlPath);
            return content.Contains("  ssl:") && !content.Contains("  #ssl:");
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
                    if (!generated)
                    {
                        Debug.WriteLine("OpenSSL generation failed, falling back to .NET X509...");
                        generated = GenerateSelfSignedCertificateFallback(domain, keyPath, crtPath);
                    }
                    if (!generated) return false;
                }

                await ConfigureRoadRunnerSSL(keyPath, crtPath);
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
            string? opensslPath = GetOpenSSLExePath();
            if (opensslPath == null) return false;

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
DNS.1 = {domain} DNS.2 = localhost IP.1 = 127.0.0.1
";
            await File.WriteAllTextAsync(configPath, configContent);

            string args = $"req -x509 -nodes -days 3650 -newkey rsa:2048 -keyout \"{keyPath}\" -out \"{crtPath}\" -config \"{configPath}\" -extensions v3_req";

            ProcessStartInfo psi = new ProcessStartInfo(opensslPath, args)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = Path.GetDirectoryName(opensslPath)
            };

            using (Process? proc = Process.Start(psi))
            {
                if (proc == null) return false;
                await proc.WaitForExitAsync();
                try { if (File.Exists(configPath)) File.Delete(configPath); } catch { }
                return proc.ExitCode == 0;
            }
        }

        private bool GenerateSelfSignedCertificateFallback(string domain, string keyPath, string crtPath)
        {
            try
            {
                using (RSA rsa = RSA.Create(2048))
                {
                    var request = new CertificateRequest(
                        $"CN={domain}",
                        rsa,
                        HashAlgorithmName.SHA256,
                        RSASignaturePadding.Pkcs1);

                    request.CertificateExtensions.Add(
                        new X509BasicConstraintsExtension(false, false, 0, false));

                    request.CertificateExtensions.Add(
                        new X509KeyUsageExtension(
                            X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment,
                            false));

                    request.CertificateExtensions.Add(
                        new X509EnhancedKeyUsageExtension(
                            new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") },
                            false));

                    var builder = new SubjectAlternativeNameBuilder();
                    builder.AddDnsName(domain);
                    builder.AddDnsName("localhost");
                    builder.AddIpAddress(System.Net.IPAddress.Loopback);
                    request.CertificateExtensions.Add(builder.Build());

                    using (X509Certificate2 cert = request.CreateSelfSigned(
                        DateTimeOffset.UtcNow.AddDays(-1),
                        DateTimeOffset.UtcNow.AddYears(10)))
                    {
                        // Export CRT
                        File.WriteAllText(crtPath, 
                            "-----BEGIN CERTIFICATE-----\n" +
                            Convert.ToBase64String(cert.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks) +
                            "\n-----END CERTIFICATE-----");

                        // Export KEY (Private Key)
                        byte[] privateKeyBytes = rsa.ExportRSAPrivateKey();
                        File.WriteAllText(keyPath,
                            "-----BEGIN RSA PRIVATE KEY-----\n" +
                            Convert.ToBase64String(privateKeyBytes, Base64FormattingOptions.InsertLineBreaks) +
                            "\n-----END RSA PRIVATE KEY-----");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Native SSL Generation failed: {ex.Message}");
                return false;
            }
        }

        private async Task ConfigureRoadRunnerSSL(string keyPath, string crtPath)
        {
            if (!File.Exists(_rrYamlPath)) return;

            string content = await File.ReadAllTextAsync(_rrYamlPath);
            
            string relKeyPath = Path.GetRelativePath(_rootPath, keyPath).Replace("\\", "/");
            string relCrtPath = Path.GetRelativePath(_rootPath, crtPath).Replace("\\", "/");

            string sslBlock = $"\n  ssl:\n    address: :8443\n    key: \"{relKeyPath}\"\n    cert: \"{relCrtPath}\"";

            if (content.Contains("http:"))
            {
                if (content.Contains("  #ssl:"))
                {
                    // Simply uncomment and update
                    content = content.Replace("  #ssl:", "  ssl:");
                }
                else if (!content.Contains("  ssl:"))
                {
                    // Inject after http:
                    content = content.Replace("http:", "http:" + sslBlock);
                }
            }

            await File.WriteAllTextAsync(_rrYamlPath, content);
        }

        public async Task<bool> DisableSSL()
        {
            if (!File.Exists(_rrYamlPath)) return false;
            string content = await File.ReadAllTextAsync(_rrYamlPath);
            content = content.Replace("  ssl:", "  #ssl:");
            await File.WriteAllTextAsync(_rrYamlPath, content);
            return true;
        }

        public async Task<bool> TrustCertificate()
        {
            string crtPath = Path.Combine(_certPath, "server.crt");
            if (!File.Exists(crtPath)) return false;

            ProcessStartInfo psi = new ProcessStartInfo("certutil", $"-addstore -f \"Root\" \"{crtPath}\"")
            {
                Verb = "runas",
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
            catch { return false; }
        }
    }
}

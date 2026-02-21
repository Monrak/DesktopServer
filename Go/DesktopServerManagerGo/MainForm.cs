using System.Runtime.InteropServices;
using System.Diagnostics;

namespace DesktopServerManagerGo;

public partial class MainForm : Form
{
    [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
    private extern static void ReleaseCapture();
    [DllImport("user32.DLL", EntryPoint = "SendMessage")]
    private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

    private string rootPath;
    private Services.RoadRunnerSSLManager sslManager;
    private bool reallyExit = false;

    // Monitoring State
    private List<float> cpuHistory = new List<float>();
    private List<float> ramHistory = new List<float>();
    private const int MaxPoints = 60;
    private PerformanceCounter? cpuCounter;
    private bool isUpdatingSSLUI = false;

    public MainForm()
    {
        InitializeComponent();
        rootPath = AppDomain.CurrentDomain.BaseDirectory;
        sslManager = new Services.RoadRunnerSSLManager(rootPath);

        // Setup Draggability
        pnlSidebar.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { ReleaseCapture(); SendMessage(Handle, 0x112, 0xf012, 0); } };
        pnlHeader.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { ReleaseCapture(); SendMessage(Handle, 0x112, 0xf012, 0); } };

        // SSL Initialization
        InitializeSSL();

        // Add Control Buttons Events
        btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
        btnClose.Click += (s, e) => Application.Exit();

        // Service Status Placeholders
        btnStartRoadRunner.Click += async (s, e) => await ToggleService("RoadRunner", lblRoadRunnerStatus, btnStartRoadRunner);
        btnStartMaria.Click += async (s, e) => await ToggleService("MariaDB", lblMariaStatus, btnStartMaria);
        btnStartPostgres.Click += async (s, e) => await ToggleService("PostgreSQL", lblPostgresStatus, btnStartPostgres);
        btnOpenPMA.Click += (s, e) => Process.Start(new ProcessStartInfo("http://localhost:8080/phpmyadmin/index.php?route=/server/databases") { UseShellExecute = true });
        btnOpenAdminer.Click += (s, e) => Process.Start(new ProcessStartInfo("http://localhost:8080/adminer.php?pgsql=127.0.0.1&username=postgres") { UseShellExecute = true });

        // Configuration and Logs
        btnViewPhpIni.Click += (s, e) => OpenInNotepad("php84\\php.ini");
        btnLogRoadRunner.Click += (s, e) => OpenInNotepad("roadrunner.log");
        btnLogMaria.Click += (s, e) => OpenInNotepad("mariadb\\data\\DESKTOP.err");
        btnLogPostgres.Click += (s, e) => OpenInNotepad("postgres\\data\\log\\postgresql.log");

        // Tray Menu Events
        menuShow.Click += (s, e) => RestoreFromTray();
        menuAbout.Click += (s, e) => new AboutForm().ShowDialog();
        menuExit.Click += (s, e) => { reallyExit = true; Application.Exit(); };
        notifyIcon.DoubleClick += (s, e) => RestoreFromTray();

        // View Switching
        btnDashboard.Click += (s, e) => ShowView("Dashboard");
        btnSettings.Click += (s, e) => ShowView("Services");
        btnLogs.Click += (s, e) => ShowView("Logs");

        // Initialize View
        ShowView("Dashboard");

        // Check for Auto-Start
        string[] args = Environment.GetCommandLineArgs();
        if (args.Contains("--autostart"))
        {
            this.Load += async (s, e) => await AutoStartAllServices();
        }

        // Auto-refresh metrics every 2 seconds
        timerMetrics = new System.Windows.Forms.Timer { Interval = 2000 };

        // Metrics Initialization
        InitializeMetrics();

        // Chart Rendering
        pnlCpuChart.Paint += (s, e) => DrawChart(e.Graphics, cpuHistory, Color.FromArgb(137, 180, 250), 100, "%");
        pnlRamChart.Paint += (s, e) => DrawChart(e.Graphics, ramHistory, Color.FromArgb(166, 227, 161), GetTotalRam(), "MB");

        this.FormClosing += MainForm_FormClosing;

        // Initialize Startup Checkbox state
        try {
            string runKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(runKey, false))
            {
                if (key != null)
                {
                    chkRunOnStartup.Checked = key.GetValue("MonrakManagerGo") != null;
                }
            }
        } catch {}
        chkRunOnStartup.CheckedChanged += ChkRunOnStartup_CheckedChanged;
    }

    private void ShowView(string viewName)
    {
        pnlServices.Visible = (viewName == "Services");
        pnlDashboard.Visible = (viewName == "Dashboard");
        pnlLogs.Visible = (viewName == "Logs");
        lblStatusHeader.Text = viewName switch {
            "Services" => "Service Control",
            "Dashboard" => "Performance Dashboard",
            "Logs" => "Installation Logs",
            _ => "Manager"
        };
        
        // Highlight active button
        btnDashboard.BackColor = (viewName == "Dashboard") ? Color.FromArgb(45, 45, 67) : Color.Transparent;
        btnSettings.BackColor = (viewName == "Services") ? Color.FromArgb(45, 45, 67) : Color.Transparent;
        btnLogs.BackColor = (viewName == "Logs") ? Color.FromArgb(45, 45, 67) : Color.Transparent;

        if (viewName == "Logs") RefreshLogs();
    }

    private PerformanceCounter? ramCounter;
    private long totalRamMb = 0;

    private void InitializeMetrics()
    {
        try
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            totalRamMb = GetTotalPhysicalMemory();
        }
        catch { /* Fallback or handle missing counter */ }

        timerMetrics.Interval = 1000;
        timerMetrics.Tick += (s, e) => UpdateMetrics();
        timerMetrics.Start();
    }

    private void UpdateMetrics()
    {
        float cpu = 0;
        try { cpu = cpuCounter?.NextValue() ?? 0; } catch { }
        
        long availableRam = 0;
        try { availableRam = (long)(ramCounter?.NextValue() ?? 0); } catch { }
        
        long usedRam = totalRamMb - availableRam;
        if (usedRam < 0) usedRam = 0;

        cpuHistory.Add(cpu);
        ramHistory.Add(usedRam);

        if (cpuHistory.Count > MaxPoints) cpuHistory.RemoveAt(0);
        if (ramHistory.Count > MaxPoints) ramHistory.RemoveAt(0);

        lblCpuValue.Text = $"{(int)cpu}%";
        lblRamValue.Text = $"{usedRam} MB / {totalRamMb} MB";

        if (pnlDashboard.Visible)
        {
            pnlCpuChart.Invalidate();
            pnlRamChart.Invalidate();
        }

        if (pnlLogs.Visible)
        {
            RefreshLogs();
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private class MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
        public MEMORYSTATUSEX() { this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX)); }
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

    private long GetTotalPhysicalMemory()
    {
        try
        {
            var msex = new MEMORYSTATUSEX();
            if (GlobalMemoryStatusEx(msex))
            {
                return (long)(msex.ullTotalPhys / 1024 / 1024);
            }
        }
        catch { }
        return 16384; // Fallback to 16GB
    }

    private long GetTotalRam()
    {
        return totalRamMb > 0 ? totalRamMb : 16384;
    }

    private void RefreshLogs()
    {
        string logPath = Path.Combine(rootPath, "install.log");
        if (File.Exists(logPath))
        {
            try
            {
                // Read with FileShare.ReadWrite to prevent locking issues
                using var fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(fs);
                var lines = reader.ReadToEnd().Split(Environment.NewLine).TakeLast(100);
                txtLogs.Text = string.Join(Environment.NewLine, lines);
                txtLogs.SelectionStart = txtLogs.TextLength;
                txtLogs.ScrollToCaret();
            }
            catch { /* File might be locked or other IO error */ }
        }
    }

    private void DrawChart(Graphics g, List<float> data, Color color, float maxValue, string unit = "%")
    {
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        float w = pnlCpuChart.Width;
        float h = pnlCpuChart.Height;
        float marginL = 40; // Room for labels
        float marginR = 10;
        float chartW = w - marginL - marginR;
        float chartH = h - 20;
        float chartY = 10;

        // 1. Draw Grid Lines (Background)
        using (var gridPen = new Pen(Color.FromArgb(40, Color.White), 1))
        {
            gridPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            for (int i = 0; i <= 4; i++)
            {
                float y = chartY + (chartH * i / 4);
                g.DrawLine(gridPen, marginL, y, marginL + chartW, y);

                // Draw labels
                var val = maxValue - (maxValue * i / 4);
                string label = unit == "%" ? $"{(int)val}%" : $"{(int)(val / 1024)}G";
                using var font = new Font("Segoe UI", 7);
                using var brush = new SolidBrush(Color.Gray);
                g.DrawString(label, font, brush, 5, y - 7);
            }
        }

        if (data.Count < 2) return;

        // 2. Prepare Points
        float stepX = chartW / (MaxPoints - 1);
        var points = new List<PointF>();
        for (int i = 0; i < data.Count; i++)
        {
            float x = marginL + (i * stepX);
            float val = Math.Min(data[i], maxValue);
            float y = chartY + chartH - (val / maxValue * chartH);
            points.Add(new PointF(x, y));
        }

        // 3. Draw Area (Fill)
        using (var areaBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
            new RectangleF(marginL, chartY, chartW, chartH),
            Color.FromArgb(80, color), Color.Transparent, 90f))
        {
            var fillPoints = new List<PointF>(points);
            fillPoints.Add(new PointF(points.Last().X, chartY + chartH));
            fillPoints.Add(new PointF(points.First().X, chartY + chartH));
            g.FillPolygon(areaBrush, fillPoints.ToArray());
        }

        // 4. Draw Line (Stroke)
        using (var linePen = new Pen(color, 2))
        {
            g.DrawLines(linePen, points.ToArray());
        }
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (!reallyExit)
        {
            e.Cancel = true;
            this.Hide();
        }
    }

    private void RestoreFromTray()
    {
        this.Show();
        this.WindowState = FormWindowState.Normal;
        this.BringToFront();
    }

    private void OpenInNotepad(string relativePath)
    {
        string fullPath = Path.Combine(rootPath, relativePath);
        
        // Handle postgresql log dynamic path if the direct file isn't found
        if (relativePath == "postgres\\data\\log\\postgresql.log" && !File.Exists(fullPath))
        {
            string logDir = Path.Combine(rootPath, "postgres", "data", "log");
            if (Directory.Exists(logDir))
            {
                var files = Directory.GetFiles(logDir, "postgresql-*.log").OrderByDescending(f => f).ToList();
                if (files.Count > 0)
                {
                    fullPath = files.First();
                }
            }
        }

        if (File.Exists(fullPath))
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "notepad.exe",
                    Arguments = $"\"{fullPath}\"",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open log file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        else
        {
            MessageBox.Show($"File not found:\n{fullPath}", "Log Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private async Task AutoStartAllServices()
    {
        // 1. Start RoadRunner
        await ToggleService("RoadRunner", lblRoadRunnerStatus, btnStartRoadRunner);
        
        // 2. Wait for RoadRunner to be active (simple delay given ToggleService waits for process start)
        await Task.Delay(2000); 

        // 3. Open Browser
        try 
        { 
            Process.Start(new ProcessStartInfo("http://localhost:8080") { UseShellExecute = true }); 
        } 
        catch { }

        // 4. Start Databases
        await ToggleService("MariaDB", lblMariaStatus, btnStartMaria);
        await ToggleService("PostgreSQL", lblPostgresStatus, btnStartPostgres);
    }

    private async Task ToggleService(string name, Label statusLabel, Button ctrlButton)
    {
        bool isStarting = ctrlButton.Text == "START";
        string exePath = "";
        string args = "";
        string procName = "";

        switch (name)
        {
            case "RoadRunner":
                exePath = Path.Combine(rootPath, "rr.exe");
                args = "serve";
                procName = "rr";
                break;
            case "MariaDB":
                exePath = Path.Combine(rootPath, "mariadb", "bin", "mariadbd.exe");
                args = "--defaults-file=\"../my.ini\"";
                procName = "mariadbd";
                break;
            case "PostgreSQL":
                exePath = Path.Combine(rootPath, "postgres", "bin", "postgres.exe");
                args = "-D \"../data\"";
                procName = "postgres";
                break;
        }

        ctrlButton.Enabled = false;
        statusLabel.Text = isStarting ? "Starting..." : "Stopping...";
        statusLabel.ForeColor = Color.Yellow;

        if (isStarting)
        {
            if (File.Exists(exePath))
            {
                try
                {
                    var psi = new ProcessStartInfo(exePath, args)
                    {
                        WorkingDirectory = Path.GetDirectoryName(exePath),
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardError = (name == "RoadRunner")
                    };



                    if (name == "RoadRunner")
                    {
                        // Redirect RR logs to file with sharing enabled
                        string logPath = Path.Combine(rootPath, "roadrunner.log");
                        psi.RedirectStandardError = true;
                        psi.RedirectStandardOutput = true;
                        psi.RedirectStandardError = true;
                        psi.RedirectStandardOutput = true;
                        var proc = Process.Start(psi);
                        if (proc != null)
                        {
                            _ = Task.Run(async () => {
                                try
                                {
                                    using var fs = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                                    using var writer = new StreamWriter(fs) { AutoFlush = true };
                                    await writer.WriteLineAsync($"--- Started at {DateTime.Now} ---");

                                    var outTask = Task.Run(async () => {
                                        while (!proc.StandardOutput.EndOfStream)
                                        {
                                            string? line = await proc.StandardOutput.ReadLineAsync();
                                            if (line != null) lock(writer) writer.WriteLine(line);
                                        }
                                    });

                                    var errTask = Task.Run(async () => {
                                        while (!proc.StandardError.EndOfStream)
                                        {
                                            string? line = await proc.StandardError.ReadLineAsync();
                                            if (line != null) lock(writer) writer.WriteLine(line);
                                        }
                                    });

                                    await Task.WhenAll(outTask, errTask);
                                }
                                catch { }
                            });
                        }
                    }
                    else
                    {
                        Process.Start(psi);
                    }
                    
                    await Task.Delay(1500); // Give it time to bind
                    statusLabel.Text = "Running";
                    statusLabel.ForeColor = Color.FromArgb(166, 227, 161);
                    ctrlButton.Text = "STOP";
                    ctrlButton.BackColor = Color.FromArgb(243, 139, 168);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to start {name}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    statusLabel.Text = "Error";
                    statusLabel.ForeColor = Color.OrangeRed;
                }
            }
            else
            {
                MessageBox.Show($"{name} binary not found at {exePath}", "Missing Component", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                statusLabel.Text = "Stopped";
                statusLabel.ForeColor = Color.FromArgb(243, 139, 168);
            }
        }

        else
        {
            // Stop logic
            foreach (var p in Process.GetProcessesByName(procName))
            {
                try { p.Kill(); } catch { }
            }



            await Task.Delay(500);
            statusLabel.Text = "Stopped";
            statusLabel.ForeColor = Color.FromArgb(243, 139, 168);
            ctrlButton.Text = "START";
            ctrlButton.BackColor = GetOriginalButtonColor(name);
        }
        ctrlButton.Enabled = true;
    }

    private Color GetOriginalButtonColor(string name)
    {
        return name switch
        {
            "RoadRunner" => Color.FromArgb(137, 180, 250),
            "MariaDB" => Color.FromArgb(166, 227, 161),
            "PostgreSQL" => Color.FromArgb(203, 166, 247),
            _ => Color.Gray
        };
    }



    private async void InitializeSSL()
    {
        bool isEnabled = await sslManager.IsSSLEnabled();
        chkEnableSSL.Checked = isEnabled;
        btnTrustCert.Visible = isEnabled;

        chkEnableSSL.CheckedChanged += async (s, e) =>
        {
            if (isUpdatingSSLUI) return;
            isUpdatingSSLUI = true;

            chkEnableSSL.Enabled = false;
            if (chkEnableSSL.Checked)
            {
                bool success = await sslManager.SetupSSL();
                if (success)
                {
                    btnTrustCert.Visible = true;
                    MessageBox.Show("SSL enabled! RoadRunner is now ready for HTTPS on port 8443.\n\nPlease restart RoadRunner to apply changes.", "SSL Configured", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to configure SSL. Check if openssl.exe exists in the openssl folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    chkEnableSSL.Checked = false;
                }
            }
            else
            {
                await sslManager.DisableSSL();
                btnTrustCert.Visible = false;
                MessageBox.Show("SSL disabled. Please restart RoadRunner.", "SSL Disabled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            chkEnableSSL.Enabled = true;
            isUpdatingSSLUI = false;
        };

        btnTrustCert.Click += async (s, e) =>
        {
            bool success = await sslManager.TrustCertificate();
            if (success)
            {
                MessageBox.Show("Local SSL Certificate trusted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Failed to trust certificate. Make sure to accept the Administrator prompt.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };
    }

    private void ChkRunOnStartup_CheckedChanged(object? sender, EventArgs e)
    {
        try
        {
            string runKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(runKey, true))
            {
                if (key != null)
                {
                    if (chkRunOnStartup.Checked)
                    {
                        key.SetValue("MonrakManagerGo", $"\"{Application.ExecutablePath}\" --autostart");
                    }
                    else
                    {
                        key.DeleteValue("MonrakManagerGo", false);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to configure startup: {ex.Message}", "Registry Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}

using System;
using System.IO;
using System.Windows.Forms;

namespace DesktopServerManagerPro;

static class Program
{
    private static string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startup.log");

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Log("========================================");
        Log($"Application Starting at {DateTime.Now}");
        Log($"Base Directory: {AppDomain.CurrentDomain.BaseDirectory}");

        try
        {
            ApplicationConfiguration.Initialize();
            Log("ApplicationConfiguration.Initialize successful.");
            
            Application.Run(new MainForm());
        }
        catch (Exception ex)
        {
            Log($"CRITICAL ERROR: {ex.Message}");
            Log($"Stack Trace: {ex.StackTrace}");
            MessageBox.Show($"Application Crash: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public static void Log(string message)
    {
        try
        {
            string logLine = $"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}";
            File.AppendAllText(logPath, logLine);
        }
        catch { }
    }
}
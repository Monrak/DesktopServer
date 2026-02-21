using System;
using System.Reflection;
using System.Windows.Forms;

namespace DesktopServerManagerGo;

public partial class AboutForm : Form
{
    public AboutForm()
    {
        InitializeComponent();
        LoadMetadata();

        btnOK.Click += (s, e) => this.Close();
    }

    private void LoadMetadata()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();

            lblProductName.Text = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "Desktop Server Go";
            lblVersion.Text = $"Version {assembly.GetName().Version?.ToString() ?? "1.0.0"} (GO EDITION)";
            lblAuthors.Text = $"Compiled by {assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "Monrak Net Co., Ltd."}";
            string desc = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "Next-Gen AI-Ready PHP Environment powered by Go & RoadRunner.";
            txtDescription.Text = desc.Replace("|", Environment.NewLine);
            lblCopyright.Text = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "Copyright © 2026 Monrak Net Protocol";
        }
        catch (Exception)
        {
            lblProductName.Text = "DesktopServer Go";
            lblVersion.Text = "Version 1.0.0";
        }
    }
}

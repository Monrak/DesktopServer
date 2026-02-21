using System.Drawing;
using System.Windows.Forms;

namespace DesktopServerManagerGo;

partial class AboutForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
        lblProductName = new Label();
        lblVersion = new Label();
        lblAuthors = new Label();
        txtDescription = new TextBox();
        lblCopyright = new Label();
        btnOK = new Button();
        panelFooter = new Panel();
        panelHeader = new Panel();
        lblHeaderTitle = new Label();
        panelFooter.SuspendLayout();
        panelHeader.SuspendLayout();
        SuspendLayout();
        // 
        // lblProductName
        // 
        lblProductName.AutoSize = true;
        lblProductName.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblProductName.ForeColor = Color.FromArgb(203, 166, 247);
        lblProductName.Location = new Point(23, 130);
        lblProductName.Name = "lblProductName";
        lblProductName.Size = new Size(572, 59);
        lblProductName.TabIndex = 1;
        lblProductName.Text = "Monrak Desktop Server Go";
        // 
        // lblVersion
        // 
        lblVersion.AutoSize = true;
        lblVersion.Font = new Font("Segoe UI", 10F);
        lblVersion.ForeColor = Color.FromArgb(147, 153, 178);
        lblVersion.Location = new Point(41, 200);
        lblVersion.Name = "lblVersion";
        lblVersion.Size = new Size(168, 37);
        lblVersion.TabIndex = 2;
        lblVersion.Text = "Version 1.0.0";
        // 
        // lblAuthors
        // 
        lblAuthors.AutoSize = true;
        lblAuthors.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblAuthors.ForeColor = Color.FromArgb(166, 227, 161);
        lblAuthors.Location = new Point(32, 530);
        lblAuthors.Name = "lblAuthors";
        lblAuthors.Size = new Size(504, 32);
        lblAuthors.TabIndex = 4;
        lblAuthors.Text = "Compiled by DesktopServer Protocol Team";
        // 
        // txtDescription
        // 
        txtDescription.BackColor = Color.FromArgb(30, 30, 46);
        txtDescription.BorderStyle = BorderStyle.None;
        txtDescription.Font = new Font("Segoe UI", 9F);
        txtDescription.ForeColor = Color.FromArgb(205, 214, 244);
        txtDescription.Location = new Point(41, 260);
        txtDescription.Multiline = true;
        txtDescription.Name = "txtDescription";
        txtDescription.ReadOnly = true;
        txtDescription.Size = new Size(654, 240);
        txtDescription.TabIndex = 3;
        txtDescription.TabStop = false;
        txtDescription.Text = "Next-Gen Local Development Environment...";
        // 
        // lblCopyright
        // 
        lblCopyright.AutoSize = true;
        lblCopyright.Font = new Font("Segoe UI", 8F);
        lblCopyright.ForeColor = Color.FromArgb(147, 153, 178);
        lblCopyright.Location = new Point(32, 570);
        lblCopyright.Name = "lblCopyright";
        lblCopyright.Size = new Size(425, 30);
        lblCopyright.TabIndex = 5;
        lblCopyright.Text = "Copyright © 2026 DesktopServer Protocol";
        // 
        // btnOK
        // 
        btnOK.BackColor = Color.FromArgb(203, 166, 247);
        btnOK.FlatAppearance.BorderSize = 0;
        btnOK.FlatStyle = FlatStyle.Flat;
        btnOK.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnOK.ForeColor = Color.FromArgb(17, 17, 27);
        btnOK.Location = new Point(598, 15);
        btnOK.Name = "btnOK";
        btnOK.Size = new Size(156, 51);
        btnOK.TabIndex = 0;
        btnOK.Text = "CLOSE";
        btnOK.UseVisualStyleBackColor = false;
        // 
        // panelFooter
        // 
        panelFooter.BackColor = Color.FromArgb(24, 24, 37);
        panelFooter.Controls.Add(btnOK);
        panelFooter.Dock = DockStyle.Bottom;
        panelFooter.Location = new Point(0, 620);
        panelFooter.Name = "panelFooter";
        panelFooter.Size = new Size(780, 83);
        panelFooter.TabIndex = 0;
        // 
        // panelHeader
        // 
        panelHeader.BackColor = Color.FromArgb(30, 30, 46);
        panelHeader.Controls.Add(lblHeaderTitle);
        panelHeader.Dock = DockStyle.Top;
        panelHeader.Location = new Point(0, 0);
        panelHeader.Name = "panelHeader";
        panelHeader.Size = new Size(780, 100);
        panelHeader.TabIndex = 21;
        // 
        // lblHeaderTitle
        // 
        lblHeaderTitle.AutoSize = true;
        lblHeaderTitle.Font = new Font("Segoe UI Black", 14F, FontStyle.Bold);
        lblHeaderTitle.ForeColor = Color.White;
        lblHeaderTitle.Location = new Point(30, 25);
        lblHeaderTitle.Name = "lblHeaderTitle";
        lblHeaderTitle.Size = new Size(158, 51);
        lblHeaderTitle.TabIndex = 0;
        lblHeaderTitle.Text = "ABOUT";
        // 
        // AboutForm
        // 
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(17, 17, 27);
        ClientSize = new Size(780, 703);
        Controls.Add(panelHeader);
        Controls.Add(panelFooter);
        Controls.Add(lblCopyright);
        Controls.Add(lblAuthors);
        Controls.Add(txtDescription);
        Controls.Add(lblVersion);
        Controls.Add(lblProductName);
        FormBorderStyle = FormBorderStyle.None;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Name = "AboutForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "About Desktop Server Go";
        panelFooter.ResumeLayout(false);
        panelHeader.ResumeLayout(false);
        panelHeader.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
    private Label lblProductName;
    private Label lblVersion;
    private Label lblAuthors;
    private TextBox txtDescription;
    private Label lblCopyright;
    private Button btnOK;
    private Panel panelFooter;
    private Panel panelHeader;
    private Label lblHeaderTitle;
}

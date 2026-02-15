using System.Drawing;
using System.Windows.Forms;

namespace DesktopServerManagerPro;

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
        panel1 = new Panel();
        pictureBox1 = new PictureBox();
        panelFooter.SuspendLayout();
        panel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        SuspendLayout();
        // 
        // lblProductName
        // 
        lblProductName.AutoSize = true;
        lblProductName.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblProductName.ForeColor = Color.FromArgb(16, 185, 129);
        lblProductName.Location = new Point(23, 139);
        lblProductName.Margin = new Padding(4, 0, 4, 0);
        lblProductName.Name = "lblProductName";
        lblProductName.Size = new Size(584, 59);
        lblProductName.TabIndex = 1;
        lblProductName.Text = "Monrak Desktop Server Pro";
        // 
        // lblVersion
        // 
        lblVersion.AutoSize = true;
        lblVersion.Font = new Font("Segoe UI", 10F);
        lblVersion.ForeColor = Color.White;
        lblVersion.Location = new Point(32, 217);
        lblVersion.Margin = new Padding(4, 0, 4, 0);
        lblVersion.Name = "lblVersion";
        lblVersion.Size = new Size(168, 37);
        lblVersion.TabIndex = 2;
        lblVersion.Text = "Version 2.0.0";
        // 
        // lblAuthors
        // 
        lblAuthors.AutoSize = true;
        lblAuthors.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblAuthors.ForeColor = Color.LightGray;
        lblAuthors.Location = new Point(32, 551);
        lblAuthors.Margin = new Padding(4, 0, 4, 0);
        lblAuthors.Name = "lblAuthors";
        lblAuthors.Size = new Size(403, 32);
        lblAuthors.TabIndex = 4;
        lblAuthors.Text = "Compiled by Monrak Net Co., Ltd.";
        // 
        // txtDescription
        // 
        txtDescription.BackColor = Color.FromArgb(17, 24, 39);
        txtDescription.BorderStyle = BorderStyle.None;
        txtDescription.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
        txtDescription.ForeColor = Color.FromArgb(200, 200, 200);
        txtDescription.Location = new Point(78, 276);
        txtDescription.Margin = new Padding(4, 0, 4, 0);
        txtDescription.Multiline = true;
        txtDescription.Name = "txtDescription";
        txtDescription.ReadOnly = true;
        txtDescription.ScrollBars = ScrollBars.Vertical;
        txtDescription.Size = new Size(654, 254);
        txtDescription.TabIndex = 3;
        txtDescription.TabStop = false;
        txtDescription.Text = "Next-Gen Local Development Environment...";
        // 
        // lblCopyright
        // 
        lblCopyright.AutoSize = true;
        lblCopyright.Font = new Font("Segoe UI", 8F);
        lblCopyright.ForeColor = Color.Gray;
        lblCopyright.Location = new Point(32, 591);
        lblCopyright.Margin = new Padding(4, 0, 4, 0);
        lblCopyright.Name = "lblCopyright";
        lblCopyright.Size = new Size(425, 30);
        lblCopyright.TabIndex = 5;
        lblCopyright.Text = "Copyright © 2026 DesktopServer Protocol";
        // 
        // btnOK
        // 
        btnOK.BackColor = Color.FromArgb(16, 185, 129);
        btnOK.FlatStyle = FlatStyle.Flat;
        btnOK.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnOK.ForeColor = Color.FromArgb(10, 10, 12);
        btnOK.Location = new Point(598, 15);
        btnOK.Margin = new Padding(4);
        btnOK.Name = "btnOK";
        btnOK.Size = new Size(156, 51);
        btnOK.TabIndex = 0;
        btnOK.Text = "CLOSE";
        btnOK.UseVisualStyleBackColor = false;
        // 
        // panelFooter
        // 
        panelFooter.BackColor = Color.FromArgb(10, 10, 12);
        panelFooter.Controls.Add(btnOK);
        panelFooter.Dock = DockStyle.Bottom;
        panelFooter.Location = new Point(0, 636);
        panelFooter.Margin = new Padding(4);
        panelFooter.Name = "panelFooter";
        panelFooter.Size = new Size(780, 83);
        panelFooter.TabIndex = 0;
        // 
        // panel1
        // 
        panel1.BackColor = Color.Black;
        panel1.Controls.Add(pictureBox1);
        panel1.Dock = DockStyle.Top;
        panel1.Location = new Point(0, 0);
        panel1.Name = "panel1";
        panel1.Size = new Size(780, 117);
        panel1.TabIndex = 25;
        // 
        // pictureBox1
        // 
        pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
        pictureBox1.Location = new Point(23, -2);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(554, 116);
        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        pictureBox1.TabIndex = 25;
        pictureBox1.TabStop = false;
        // 
        // AboutForm
        // 
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(17, 24, 39);
        ClientSize = new Size(780, 719);
        Controls.Add(panel1);
        Controls.Add(panelFooter);
        Controls.Add(lblCopyright);
        Controls.Add(lblAuthors);
        Controls.Add(txtDescription);
        Controls.Add(lblVersion);
        Controls.Add(lblProductName);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(4);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AboutForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "About DesktopServer";
        panelFooter.ResumeLayout(false);
        panel1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
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
    private Panel panel1;
    private PictureBox pictureBox1;
}

namespace DesktopServerSetupPro;

partial class MainForm
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

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        lblPathHeader = new Label();
        txtInstallPath = new TextBox();
        btnBrowse = new Button();
        lblDescription = new Label();
        lblStatus = new Label();
        progressBar = new ProgressBar();
        txtLog = new TextBox();
        btnStart = new Button();
        checkBox1 = new CheckBox();
        panel1 = new Panel();
        pictureBox1 = new PictureBox();
        panel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        SuspendLayout();
        // 
        // lblPathHeader
        // 
        lblPathHeader.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
        lblPathHeader.Location = new Point(29, 164);
        lblPathHeader.Margin = new Padding(6, 0, 6, 0);
        lblPathHeader.Name = "lblPathHeader";
        lblPathHeader.Size = new Size(1003, 49);
        lblPathHeader.TabIndex = 1;
        lblPathHeader.Text = "Choose Installation Path:";
        // 
        // txtInstallPath
        // 
        txtInstallPath.Location = new Point(29, 225);
        txtInstallPath.Margin = new Padding(6);
        txtInstallPath.Name = "txtInstallPath";
        txtInstallPath.Size = new Size(814, 39);
        txtInstallPath.TabIndex = 6;
        txtInstallPath.Text = "C:\\MonrakDesktopServerPro";
        // 
        // btnBrowse
        // 
        btnBrowse.Location = new Point(861, 225);
        btnBrowse.Margin = new Padding(6);
        btnBrowse.Name = "btnBrowse";
        btnBrowse.Size = new Size(167, 49);
        btnBrowse.TabIndex = 7;
        btnBrowse.Text = "Browse...";
        btnBrowse.UseVisualStyleBackColor = true;
        // 
        // lblDescription
        // 
        lblDescription.Location = new Point(29, 286);
        lblDescription.Margin = new Padding(6, 0, 6, 0);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(1003, 45);
        lblDescription.TabIndex = 5;
        lblDescription.Text = "This will install Apache, PHP, MySQL, and a 'www' folder for your projects.";
        // 
        // lblStatus
        // 
        lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
        lblStatus.ForeColor = Color.DarkBlue;
        lblStatus.Location = new Point(29, 329);
        lblStatus.Margin = new Padding(6, 0, 6, 0);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(1003, 49);
        lblStatus.TabIndex = 4;
        lblStatus.Text = "Ready to install (Offline Mode)";
        // 
        // progressBar
        // 
        progressBar.Location = new Point(29, 384);
        progressBar.Margin = new Padding(6);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(1012, 53);
        progressBar.TabIndex = 3;
        // 
        // txtLog
        // 
        txtLog.BackColor = SystemColors.Window;
        txtLog.Font = new Font("Consolas", 8F);
        txtLog.Location = new Point(29, 450);
        txtLog.Margin = new Padding(6);
        txtLog.Multiline = true;
        txtLog.Name = "txtLog";
        txtLog.ReadOnly = true;
        txtLog.ScrollBars = ScrollBars.Vertical;
        txtLog.Size = new Size(1009, 315);
        txtLog.TabIndex = 2;
        // 
        // btnStart
        // 
        btnStart.BackColor = Color.Black;
        btnStart.FlatStyle = FlatStyle.Flat;
        btnStart.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        btnStart.ForeColor = Color.White;
        btnStart.Location = new Point(638, 789);
        btnStart.Margin = new Padding(6);
        btnStart.Name = "btnStart";
        btnStart.Size = new Size(394, 107);
        btnStart.TabIndex = 0;
        btnStart.Text = "START SETUP";
        btnStart.UseVisualStyleBackColor = false;
        // 
        // checkBox1
        // 
        checkBox1.AutoSize = true;
        checkBox1.Checked = true;
        checkBox1.CheckState = CheckState.Checked;
        checkBox1.Location = new Point(29, 809);
        checkBox1.Margin = new Padding(6);
        checkBox1.Name = "checkBox1";
        checkBox1.Size = new Size(405, 36);
        checkBox1.TabIndex = 1;
        checkBox1.Text = "Open Monrak Desktop Server Pro";
        checkBox1.UseVisualStyleBackColor = true;
        // 
        // panel1
        // 
        panel1.BackColor = Color.Black;
        panel1.Controls.Add(pictureBox1);
        panel1.Dock = DockStyle.Top;
        panel1.Location = new Point(0, 0);
        panel1.Name = "panel1";
        panel1.Size = new Size(1085, 140);
        panel1.TabIndex = 8;
        // 
        // pictureBox1
        // 
        pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
        pictureBox1.Location = new Point(12, 3);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(740, 137);
        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        pictureBox1.TabIndex = 0;
        pictureBox1.TabStop = false;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1085, 937);
        Controls.Add(panel1);
        Controls.Add(checkBox1);
        Controls.Add(lblPathHeader);
        Controls.Add(txtInstallPath);
        Controls.Add(btnBrowse);
        Controls.Add(lblDescription);
        Controls.Add(lblStatus);
        Controls.Add(progressBar);
        Controls.Add(txtLog);
        Controls.Add(btnStart);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(6);
        MaximizeBox = false;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Monrak Desktop Server Pro - Setup";
        panel1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private Label lblPathHeader;
    private TextBox txtInstallPath;
    private Button btnBrowse;
    private Label lblDescription;
    private Label lblStatus;
    private ProgressBar progressBar;
    private TextBox txtLog;
    private Button btnStart;
    private CheckBox checkBox1;
    private Panel panel1;
    private PictureBox pictureBox1;
}

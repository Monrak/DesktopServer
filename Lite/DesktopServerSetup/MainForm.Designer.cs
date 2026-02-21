namespace DesktopServerSetup;

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
        lblPathHeader.AutoSize = true;
        lblPathHeader.Font = new Font("Segoe UI", 9F);
        lblPathHeader.Location = new Point(20, 110);
        lblPathHeader.Name = "lblPathHeader";
        lblPathHeader.Size = new Size(137, 15);
        lblPathHeader.TabIndex = 1;
        lblPathHeader.Text = "Choose Installation Path:";
        // 
        // txtInstallPath
        // 
        txtInstallPath.BackColor = Color.FromArgb(240, 240, 240);
        txtInstallPath.BorderStyle = BorderStyle.FixedSingle;
        txtInstallPath.Location = new Point(20, 130);
        txtInstallPath.Name = "txtInstallPath";
        txtInstallPath.Size = new Size(400, 23);
        txtInstallPath.TabIndex = 6;
        txtInstallPath.Text = "C:\\DesktopServerLite";
        // 
        // btnBrowse
        // 
        btnBrowse.Location = new Point(430, 128);
        btnBrowse.Name = "btnBrowse";
        btnBrowse.Size = new Size(100, 27);
        btnBrowse.TabIndex = 7;
        btnBrowse.Text = "Browse...";
        btnBrowse.UseVisualStyleBackColor = true;
        // 
        // lblDescription
        // 
        lblDescription.Font = new Font("Segoe UI", 8F);
        lblDescription.ForeColor = Color.DimGray;
        lblDescription.Location = new Point(20, 155);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(510, 20);
        lblDescription.TabIndex = 5;
        lblDescription.Text = "Installs Apache, PHP, MySQL, and a project folder.";
        // 
        // lblStatus
        // 
        lblStatus.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
        lblStatus.ForeColor = Color.DarkBlue;
        lblStatus.Location = new Point(20, 415);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(350, 20);
        lblStatus.TabIndex = 4;
        lblStatus.Text = "Ready to install";
        // 
        // progressBar
        // 
        progressBar.Location = new Point(20, 400);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(350, 10);
        progressBar.TabIndex = 3;
        // 
        // txtLog
        // 
        txtLog.BackColor = SystemColors.Window;
        txtLog.Font = new Font("Consolas", 8F);
        txtLog.Location = new Point(20, 185);
        txtLog.Multiline = true;
        txtLog.Name = "txtLog";
        txtLog.ReadOnly = true;
        txtLog.ScrollBars = ScrollBars.Vertical;
        txtLog.Size = new Size(510, 200);
        txtLog.TabIndex = 2;
        // 
        // btnStart
        // 
        btnStart.BackColor = Color.Black;
        btnStart.FlatAppearance.BorderSize = 0;
        btnStart.FlatStyle = FlatStyle.Flat;
        btnStart.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        btnStart.ForeColor = Color.White;
        btnStart.Location = new Point(380, 400);
        btnStart.Name = "btnStart";
        btnStart.Size = new Size(150, 40);
        btnStart.TabIndex = 0;
        btnStart.Text = "START SETUP";
        btnStart.UseVisualStyleBackColor = false;
        // 
        // checkBox1
        // 
        checkBox1.AutoSize = true;
        checkBox1.Checked = true;
        checkBox1.CheckState = CheckState.Checked;
        checkBox1.Font = new Font("Segoe UI", 8F);
        checkBox1.Location = new Point(20, 435);
        checkBox1.Name = "checkBox1";
        checkBox1.Size = new Size(179, 17);
        checkBox1.TabIndex = 1;
        checkBox1.Text = "Open Monrak Desktop Server Lite";
        checkBox1.UseVisualStyleBackColor = true;
        // 
        // panel1
        // 
        panel1.BackColor = Color.Black;
        panel1.Controls.Add(pictureBox1);
        panel1.Dock = DockStyle.Top;
        panel1.Location = new Point(0, 0);
        panel1.Name = "panel1";
        panel1.Size = new Size(550, 100);
        panel1.TabIndex = 8;
        // 
        // pictureBox1
        // 
        pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
        pictureBox1.Location = new Point(10, 5);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(500, 90);
        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        pictureBox1.TabIndex = 0;
        pictureBox1.TabStop = false;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        ClientSize = new Size(550, 465);
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
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Monrak Desktop Server Lite - Setup";
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

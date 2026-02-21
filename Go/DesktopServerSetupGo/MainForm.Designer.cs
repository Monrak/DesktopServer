namespace DesktopServerSetupGo;

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

    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        lblMain = new Label();
        pnlHeader = new Panel();
        btnMinimize = new Button();
        btnClose = new Button();
        btnInstall = new Button();
        progressBar = new ProgressBar();
        lblStatus = new Label();
        txtInstallPath = new TextBox();
        btnBrowse = new Button();
        txtLog = new RichTextBox();
        lblPath = new Label();
        pnlHeader.SuspendLayout();
        SuspendLayout();
        // 
        // lblMain
        // 
        lblMain.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold);
        lblMain.ForeColor = Color.FromArgb(203, 166, 247);
        lblMain.Location = new Point(0, 0);
        lblMain.Margin = new Padding(6, 0, 6, 0);
        lblMain.Name = "lblMain";
        lblMain.Size = new Size(1015, 107);
        lblMain.TabIndex = 0;
        lblMain.Text = "Monrak Desktop Server Go!";
        lblMain.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // pnlHeader
        // 
        pnlHeader.BackColor = Color.FromArgb(30, 30, 46);
        pnlHeader.Controls.Add(btnMinimize);
        pnlHeader.Controls.Add(btnClose);
        pnlHeader.Controls.Add(lblMain);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(1021, 107);
        pnlHeader.TabIndex = 10;
        // 
        // btnMinimize
        // 
        btnMinimize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnMinimize.Cursor = Cursors.Hand;
        btnMinimize.FlatAppearance.BorderSize = 0;
        btnMinimize.FlatStyle = FlatStyle.Flat;
        btnMinimize.Font = new Font("Segoe UI", 12F);
        btnMinimize.ForeColor = Color.White;
        btnMinimize.Location = new Point(1727, 21);
        btnMinimize.Margin = new Padding(6);
        btnMinimize.Name = "btnMinimize";
        btnMinimize.Size = new Size(56, 64);
        btnMinimize.TabIndex = 9;
        btnMinimize.Text = "–";
        btnMinimize.UseVisualStyleBackColor = true;
        // 
        // btnClose
        // 
        btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnClose.Cursor = Cursors.Hand;
        btnClose.FlatAppearance.BorderSize = 0;
        btnClose.FlatStyle = FlatStyle.Flat;
        btnClose.Font = new Font("Segoe UI", 12F);
        btnClose.ForeColor = Color.White;
        btnClose.Location = new Point(1786, 21);
        btnClose.Margin = new Padding(6);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(56, 64);
        btnClose.TabIndex = 8;
        btnClose.Text = "✕";
        btnClose.UseVisualStyleBackColor = true;
        // 
        // btnInstall
        // 
        btnInstall.BackColor = Color.FromArgb(137, 180, 250);
        btnInstall.FlatAppearance.BorderSize = 0;
        btnInstall.FlatStyle = FlatStyle.Flat;
        btnInstall.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        btnInstall.ForeColor = Color.FromArgb(17, 17, 27);
        btnInstall.Location = new Point(762, 612);
        btnInstall.Margin = new Padding(6);
        btnInstall.Name = "btnInstall";
        btnInstall.Size = new Size(223, 85);
        btnInstall.TabIndex = 0;
        btnInstall.Text = "INSTALL";
        btnInstall.UseVisualStyleBackColor = false;
        // 
        // progressBar
        // 
        progressBar.Location = new Point(38, 612);
        progressBar.Margin = new Padding(6);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(706, 21);
        progressBar.TabIndex = 2;
        // 
        // lblStatus
        // 
        lblStatus.Font = new Font("Segoe UI", 8F);
        lblStatus.ForeColor = Color.FromArgb(147, 153, 178);
        lblStatus.Location = new Point(38, 644);
        lblStatus.Margin = new Padding(6, 0, 6, 0);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(706, 53);
        lblStatus.TabIndex = 3;
        lblStatus.Text = "Ready to install";
        lblStatus.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // txtInstallPath
        // 
        txtInstallPath.BackColor = Color.FromArgb(45, 45, 67);
        txtInstallPath.BorderStyle = BorderStyle.None;
        txtInstallPath.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
        txtInstallPath.ForeColor = Color.White;
        txtInstallPath.Location = new Point(37, 171);
        txtInstallPath.Margin = new Padding(6);
        txtInstallPath.Name = "txtInstallPath";
        txtInstallPath.Size = new Size(743, 32);
        txtInstallPath.TabIndex = 4;
        txtInstallPath.TabStop = false;
        txtInstallPath.Text = "C:\\DesktopServerGo";
        // 
        // btnBrowse
        // 
        btnBrowse.BackColor = Color.FromArgb(69, 71, 90);
        btnBrowse.FlatAppearance.BorderSize = 0;
        btnBrowse.FlatStyle = FlatStyle.Flat;
        btnBrowse.Font = new Font("Segoe UI", 7.125F, FontStyle.Regular, GraphicsUnit.Point, 0);
        btnBrowse.ForeColor = Color.White;
        btnBrowse.Location = new Point(799, 171);
        btnBrowse.Margin = new Padding(6);
        btnBrowse.Name = "btnBrowse";
        btnBrowse.Size = new Size(186, 36);
        btnBrowse.TabIndex = 5;
        btnBrowse.Text = "BROWSE...";
        btnBrowse.UseVisualStyleBackColor = false;
        // 
        // txtLog
        // 
        txtLog.BackColor = Color.FromArgb(24, 24, 37);
        txtLog.BorderStyle = BorderStyle.None;
        txtLog.Font = new Font("Consolas", 8F);
        txtLog.ForeColor = Color.FromArgb(166, 173, 200);
        txtLog.Location = new Point(37, 219);
        txtLog.Margin = new Padding(6);
        txtLog.Name = "txtLog";
        txtLog.ReadOnly = true;
        txtLog.Size = new Size(947, 363);
        txtLog.TabIndex = 7;
        txtLog.Text = "";
        // 
        // lblPath
        // 
        lblPath.AutoSize = true;
        lblPath.Font = new Font("Segoe UI", 8F);
        lblPath.ForeColor = Color.FromArgb(147, 153, 178);
        lblPath.Location = new Point(37, 128);
        lblPath.Margin = new Padding(6, 0, 6, 0);
        lblPath.Name = "lblPath";
        lblPath.Size = new Size(168, 30);
        lblPath.TabIndex = 6;
        lblPath.Text = "Installation Path:";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(30, 30, 46);
        ClientSize = new Size(1021, 736);
        Controls.Add(txtLog);
        Controls.Add(lblPath);
        Controls.Add(btnBrowse);
        Controls.Add(txtInstallPath);
        Controls.Add(lblStatus);
        Controls.Add(progressBar);
        Controls.Add(btnInstall);
        Controls.Add(pnlHeader);
        FormBorderStyle = FormBorderStyle.None;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(6);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Monrak Desktop Server Go! Setup";
        pnlHeader.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    private Panel pnlHeader;
    private Label lblMain;
    private Button btnInstall;
    private ProgressBar progressBar;
    private Label lblStatus;
    private TextBox txtInstallPath;
    private Button btnBrowse;
    private RichTextBox txtLog;
    private Label lblPath;
    private Button btnMinimize;
    private Button btnClose;
}

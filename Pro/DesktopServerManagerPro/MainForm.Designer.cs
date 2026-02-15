namespace DesktopServerManagerPro;

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
        components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        labelApache = new Label();
        lblApacheStatus = new Label();
        btnStartApache = new Button();
        btnStopApache = new Button();
        btnEditApacheConfig = new Button();
        btnOpenApacheLog = new Button();
        labelMySQL = new Label();
        lblMySQLStatus = new Label();
        btnStartMySQL = new Button();
        btnStopMySQL = new Button();
        btnEditMySQLConfig = new Button();
        btnOpenMySQLLog = new Button();
        labelLogHeader = new Label();
        txtLog = new TextBox();
        btnOpenWeb = new Button();
        btnOpenPMA = new Button();
        btnEditPHPConfig = new Button();
        notifyIcon = new NotifyIcon(components);
        trayMenu = new ContextMenuStrip(components);
        menuShow = new ToolStripMenuItem();
        menuAbout = new ToolStripMenuItem();
        toolStripMenuItem1 = new ToolStripSeparator();
        menuExit = new ToolStripMenuItem();
        chkStartWithWindows = new CheckBox();
        btnEditPMAConfig = new Button();
        cboPhpVersion = new ComboBox();
        btnSwitchPhp = new Button();
        btnTerminal = new Button();
        pictureBox1 = new PictureBox();
        chkEnableSSL = new CheckBox();
        btnTrustCert = new Button();
        pictureBox2 = new PictureBox();
        label1 = new Label();
        trayMenu.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
        SuspendLayout();
        // 
        // labelApache
        // 
        labelApache.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        labelApache.Location = new Point(40, 298);
        labelApache.Margin = new Padding(6, 0, 6, 0);
        labelApache.Name = "labelApache";
        labelApache.Size = new Size(267, 53);
        labelApache.TabIndex = 1;
        labelApache.Text = "Apache HTTP:";
        labelApache.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // lblApacheStatus
        // 
        lblApacheStatus.ForeColor = Color.Gray;
        lblApacheStatus.Location = new Point(271, 298);
        lblApacheStatus.Margin = new Padding(6, 0, 6, 0);
        lblApacheStatus.Name = "lblApacheStatus";
        lblApacheStatus.Size = new Size(200, 53);
        lblApacheStatus.TabIndex = 2;
        lblApacheStatus.Text = "Checking...";
        lblApacheStatus.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // btnStartApache
        // 
        btnStartApache.Location = new Point(483, 297);
        btnStartApache.Margin = new Padding(6);
        btnStartApache.Name = "btnStartApache";
        btnStartApache.Size = new Size(143, 55);
        btnStartApache.TabIndex = 3;
        btnStartApache.Text = "Start";
        // 
        // btnStopApache
        // 
        btnStopApache.Location = new Point(637, 297);
        btnStopApache.Margin = new Padding(6);
        btnStopApache.Name = "btnStopApache";
        btnStopApache.Size = new Size(128, 55);
        btnStopApache.TabIndex = 4;
        btnStopApache.Text = "Stop";
        // 
        // btnEditApacheConfig
        // 
        btnEditApacheConfig.Location = new Point(776, 297);
        btnEditApacheConfig.Margin = new Padding(6);
        btnEditApacheConfig.Name = "btnEditApacheConfig";
        btnEditApacheConfig.Size = new Size(214, 55);
        btnEditApacheConfig.TabIndex = 13;
        btnEditApacheConfig.Text = "Apache Config";
        // 
        // btnOpenApacheLog
        // 
        btnOpenApacheLog.Location = new Point(1006, 297);
        btnOpenApacheLog.Margin = new Padding(6);
        btnOpenApacheLog.Name = "btnOpenApacheLog";
        btnOpenApacheLog.Size = new Size(156, 55);
        btnOpenApacheLog.TabIndex = 15;
        btnOpenApacheLog.Text = "Logs";
        // 
        // labelMySQL
        // 
        labelMySQL.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        labelMySQL.Location = new Point(40, 361);
        labelMySQL.Margin = new Padding(6, 0, 6, 0);
        labelMySQL.Name = "labelMySQL";
        labelMySQL.Size = new Size(267, 53);
        labelMySQL.TabIndex = 5;
        labelMySQL.Text = "MySQL 8.0.27:";
        labelMySQL.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // lblMySQLStatus
        // 
        lblMySQLStatus.ForeColor = Color.Gray;
        lblMySQLStatus.Location = new Point(271, 361);
        lblMySQLStatus.Margin = new Padding(6, 0, 6, 0);
        lblMySQLStatus.Name = "lblMySQLStatus";
        lblMySQLStatus.Size = new Size(200, 53);
        lblMySQLStatus.TabIndex = 6;
        lblMySQLStatus.Text = "Checking...";
        lblMySQLStatus.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // btnStartMySQL
        // 
        btnStartMySQL.Location = new Point(483, 360);
        btnStartMySQL.Margin = new Padding(6);
        btnStartMySQL.Name = "btnStartMySQL";
        btnStartMySQL.Size = new Size(143, 55);
        btnStartMySQL.TabIndex = 7;
        btnStartMySQL.Text = "Start";
        // 
        // btnStopMySQL
        // 
        btnStopMySQL.Location = new Point(637, 360);
        btnStopMySQL.Margin = new Padding(6);
        btnStopMySQL.Name = "btnStopMySQL";
        btnStopMySQL.Size = new Size(128, 55);
        btnStopMySQL.TabIndex = 8;
        btnStopMySQL.Text = "Stop";
        // 
        // btnEditMySQLConfig
        // 
        btnEditMySQLConfig.Location = new Point(776, 360);
        btnEditMySQLConfig.Margin = new Padding(6);
        btnEditMySQLConfig.Name = "btnEditMySQLConfig";
        btnEditMySQLConfig.Size = new Size(214, 55);
        btnEditMySQLConfig.TabIndex = 14;
        btnEditMySQLConfig.Text = "MySQL Config";
        // 
        // btnOpenMySQLLog
        // 
        btnOpenMySQLLog.Location = new Point(1006, 360);
        btnOpenMySQLLog.Margin = new Padding(6);
        btnOpenMySQLLog.Name = "btnOpenMySQLLog";
        btnOpenMySQLLog.Size = new Size(156, 55);
        btnOpenMySQLLog.TabIndex = 16;
        btnOpenMySQLLog.Text = "Logs";
        // 
        // labelLogHeader
        // 
        labelLogHeader.AutoSize = true;
        labelLogHeader.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        labelLogHeader.Location = new Point(45, 443);
        labelLogHeader.Margin = new Padding(6, 0, 6, 0);
        labelLogHeader.Name = "labelLogHeader";
        labelLogHeader.Size = new Size(157, 32);
        labelLogHeader.TabIndex = 9;
        labelLogHeader.Text = "Activity Log:";
        // 
        // txtLog
        // 
        txtLog.BackColor = Color.GhostWhite;
        txtLog.Font = new Font("Consolas", 8F);
        txtLog.Location = new Point(42, 481);
        txtLog.Margin = new Padding(6);
        txtLog.Multiline = true;
        txtLog.Name = "txtLog";
        txtLog.ReadOnly = true;
        txtLog.ScrollBars = ScrollBars.Vertical;
        txtLog.Size = new Size(1120, 237);
        txtLog.TabIndex = 10;
        // 
        // btnOpenWeb
        // 
        btnOpenWeb.Location = new Point(39, 730);
        btnOpenWeb.Margin = new Padding(6);
        btnOpenWeb.Name = "btnOpenWeb";
        btnOpenWeb.Size = new Size(223, 75);
        btnOpenWeb.TabIndex = 11;
        btnOpenWeb.Text = "Go to Website";
        // 
        // btnOpenPMA
        // 
        btnOpenPMA.Location = new Point(273, 730);
        btnOpenPMA.Margin = new Padding(6);
        btnOpenPMA.Name = "btnOpenPMA";
        btnOpenPMA.Size = new Size(280, 75);
        btnOpenPMA.TabIndex = 12;
        btnOpenPMA.Text = "phpMyAdmin";
        // 
        // btnEditPHPConfig
        // 
        btnEditPHPConfig.Location = new Point(926, 730);
        btnEditPHPConfig.Margin = new Padding(6);
        btnEditPHPConfig.Name = "btnEditPHPConfig";
        btnEditPHPConfig.Size = new Size(236, 75);
        btnEditPHPConfig.TabIndex = 17;
        btnEditPHPConfig.Text = "PHP Config";
        btnEditPHPConfig.Click += btnEditPHPConfig_Click;
        // 
        // notifyIcon
        // 
        notifyIcon.ContextMenuStrip = trayMenu;
        notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
        notifyIcon.Text = "Monrak Desktop Server Pro";
        notifyIcon.Visible = true;
        // 
        // trayMenu
        // 
        trayMenu.ImageScalingSize = new Size(32, 32);
        trayMenu.Items.AddRange(new ToolStripItem[] { menuShow, menuAbout, toolStripMenuItem1, menuExit });
        trayMenu.Name = "trayMenu";
        trayMenu.Size = new Size(382, 124);
        // 
        // menuShow
        // 
        menuShow.Name = "menuShow";
        menuShow.Size = new Size(381, 38);
        menuShow.Text = "Monrak Desktop Server Pro";
        // 
        // menuAbout
        // 
        menuAbout.Name = "menuAbout";
        menuAbout.Size = new Size(381, 38);
        menuAbout.Text = "About Us";
        menuAbout.Click += menuAbout_Click;
        // 
        // toolStripMenuItem1
        // 
        toolStripMenuItem1.Name = "toolStripMenuItem1";
        toolStripMenuItem1.Size = new Size(378, 6);
        // 
        // menuExit
        // 
        menuExit.Name = "menuExit";
        menuExit.Size = new Size(381, 38);
        menuExit.Text = "Exit";
        // 
        // chkStartWithWindows
        // 
        chkStartWithWindows.AutoSize = true;
        chkStartWithWindows.Checked = true;
        chkStartWithWindows.CheckState = CheckState.Checked;
        chkStartWithWindows.Location = new Point(912, 240);
        chkStartWithWindows.Margin = new Padding(6);
        chkStartWithWindows.Name = "chkStartWithWindows";
        chkStartWithWindows.Size = new Size(250, 36);
        chkStartWithWindows.TabIndex = 18;
        chkStartWithWindows.Text = "Start with Windows";
        chkStartWithWindows.UseVisualStyleBackColor = true;
        // 
        // btnEditPMAConfig
        // 
        btnEditPMAConfig.Location = new Point(562, 730);
        btnEditPMAConfig.Name = "btnEditPMAConfig";
        btnEditPMAConfig.Size = new Size(355, 75);
        btnEditPMAConfig.TabIndex = 18;
        btnEditPMAConfig.Text = "phpMyAdmin Config";
        // 
        // cboPhpVersion
        // 
        cboPhpVersion.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPhpVersion.Font = new Font("Segoe UI", 10.875F, FontStyle.Regular, GraphicsUnit.Point, 0);
        cboPhpVersion.FormattingEnabled = true;
        cboPhpVersion.Location = new Point(271, 234);
        cboPhpVersion.Margin = new Padding(6);
        cboPhpVersion.Name = "cboPhpVersion";
        cboPhpVersion.Size = new Size(200, 48);
        cboPhpVersion.TabIndex = 20;
        // 
        // btnSwitchPhp
        // 
        btnSwitchPhp.Location = new Point(483, 232);
        btnSwitchPhp.Margin = new Padding(6);
        btnSwitchPhp.Name = "btnSwitchPhp";
        btnSwitchPhp.Size = new Size(143, 53);
        btnSwitchPhp.TabIndex = 23;
        btnSwitchPhp.Text = "Switch";
        btnSwitchPhp.UseVisualStyleBackColor = true;
        // 
        // btnTerminal
        // 
        btnTerminal.Location = new Point(637, 231);
        btnTerminal.Margin = new Padding(6);
        btnTerminal.Name = "btnTerminal";
        btnTerminal.Size = new Size(223, 55);
        btnTerminal.TabIndex = 21;
        btnTerminal.Text = "Open Terminal";
        btnTerminal.UseVisualStyleBackColor = true;
        // 
        // pictureBox1
        // 
        pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
        pictureBox1.Location = new Point(0, 0);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(644, 131);
        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        pictureBox1.TabIndex = 25;
        pictureBox1.TabStop = false;
        // 
        // chkEnableSSL
        // 
        chkEnableSSL.AutoSize = true;
        chkEnableSSL.Location = new Point(483, 431);
        chkEnableSSL.Name = "chkEnableSSL";
        chkEnableSSL.Size = new Size(161, 36);
        chkEnableSSL.TabIndex = 26;
        chkEnableSSL.Text = "Enable SSL";
        chkEnableSSL.UseVisualStyleBackColor = true;
        // 
        // btnTrustCert
        // 
        btnTrustCert.Location = new Point(650, 424);
        btnTrustCert.Name = "btnTrustCert";
        btnTrustCert.Size = new Size(340, 48);
        btnTrustCert.TabIndex = 27;
        btnTrustCert.Text = "Trust Certificate";
        btnTrustCert.UseVisualStyleBackColor = true;
        btnTrustCert.Visible = false;
        // 
        // pictureBox2
        // 
        pictureBox2.BackColor = Color.Black;
        pictureBox2.Dock = DockStyle.Top;
        pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
        pictureBox2.Location = new Point(0, 0);
        pictureBox2.Name = "pictureBox2";
        pictureBox2.Size = new Size(1205, 186);
        pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBox2.TabIndex = 28;
        pictureBox2.TabStop = false;
        // 
        // label1
        // 
        label1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        label1.Location = new Point(39, 232);
        label1.Margin = new Padding(6, 0, 6, 0);
        label1.Name = "label1";
        label1.Size = new Size(267, 53);
        label1.TabIndex = 1;
        label1.Text = "PHP Version:";
        label1.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1205, 870);
        Controls.Add(pictureBox2);
        Controls.Add(labelApache);
        Controls.Add(lblApacheStatus);
        Controls.Add(btnStartApache);
        Controls.Add(btnStopApache);
        Controls.Add(btnEditApacheConfig);
        Controls.Add(btnOpenApacheLog);
        Controls.Add(labelMySQL);
        Controls.Add(lblMySQLStatus);
        Controls.Add(btnStartMySQL);
        Controls.Add(btnStopMySQL);
        Controls.Add(btnEditMySQLConfig);
        Controls.Add(btnOpenMySQLLog);
        Controls.Add(labelLogHeader);
        Controls.Add(txtLog);
        Controls.Add(btnOpenWeb);
        Controls.Add(btnOpenPMA);
        Controls.Add(btnEditPHPConfig);
        Controls.Add(btnEditPMAConfig);
        Controls.Add(chkStartWithWindows);
        Controls.Add(cboPhpVersion);
        Controls.Add(btnSwitchPhp);
        Controls.Add(btnTerminal);
        Controls.Add(chkEnableSSL);
        Controls.Add(btnTrustCert);
        Controls.Add(label1);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(6);
        MaximizeBox = false;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Monrak Desktop Server Pro - ACTIVE";
        Load += MainForm_Load;
        trayMenu.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private Label labelApache;
    private Label lblApacheStatus;
    private Button btnStartApache;
    private Button btnStopApache;
    private Button btnEditApacheConfig;
    private Button btnOpenApacheLog;
    private Label labelMySQL;
    private Label lblMySQLStatus;
    private Button btnStartMySQL;
    private Button btnStopMySQL;
    private Button btnEditMySQLConfig;
    private Button btnOpenMySQLLog;
    private Button btnEditPHPConfig;
    private Label labelLogHeader;
    private TextBox txtLog;
    private Button btnOpenWeb;
    private Button btnOpenPMA;
    private Button btnEditPMAConfig;
    private CheckBox chkStartWithWindows;
    private NotifyIcon notifyIcon;
    private ContextMenuStrip trayMenu;
    private ToolStripMenuItem menuShow;
    private ToolStripMenuItem menuAbout;
    private ToolStripMenuItem menuExit;
    private ToolStripSeparator toolStripMenuItem1;
    private ComboBox cboPhpVersion;
    private Button btnSwitchPhp;
    private Button btnTerminal;
    private Label labelPhpVersion;
    private Panel panel1;
    private PictureBox pictureBox1;
    private CheckBox chkEnableSSL;
    private Button btnTrustCert;
    private PictureBox pictureBox2;
    private Label label1;
}

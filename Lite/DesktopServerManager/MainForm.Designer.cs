namespace DesktopServerManager;

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
        aboutUsToolStripMenuItem = new ToolStripMenuItem();
        toolStripMenuItem1 = new ToolStripSeparator();
        menuExit = new ToolStripMenuItem();
        chkStartWithWindows = new CheckBox();
        btnEditPMAConfig = new Button();
        panel1 = new Panel();
        pictureBox1 = new PictureBox();
        trayMenu.SuspendLayout();
        panel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        SuspendLayout();
        // 
        // labelApache
        // 
        labelApache.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        labelApache.Location = new Point(47, 158);
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
        lblApacheStatus.Location = new Point(326, 158);
        lblApacheStatus.Margin = new Padding(6, 0, 6, 0);
        lblApacheStatus.Name = "lblApacheStatus";
        lblApacheStatus.Size = new Size(175, 53);
        lblApacheStatus.TabIndex = 2;
        lblApacheStatus.Text = "Checking...";
        lblApacheStatus.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // btnStartApache
        // 
        btnStartApache.Location = new Point(512, 153);
        btnStartApache.Margin = new Padding(6);
        btnStartApache.Name = "btnStartApache";
        btnStartApache.Size = new Size(121, 60);
        btnStartApache.TabIndex = 3;
        btnStartApache.Text = "Start";
        // 
        // btnStopApache
        // 
        btnStopApache.Location = new Point(644, 153);
        btnStopApache.Margin = new Padding(6);
        btnStopApache.Name = "btnStopApache";
        btnStopApache.Size = new Size(128, 60);
        btnStopApache.TabIndex = 4;
        btnStopApache.Text = "Stop";
        // 
        // btnEditApacheConfig
        // 
        btnEditApacheConfig.Location = new Point(783, 153);
        btnEditApacheConfig.Margin = new Padding(6);
        btnEditApacheConfig.Name = "btnEditApacheConfig";
        btnEditApacheConfig.Size = new Size(214, 60);
        btnEditApacheConfig.TabIndex = 13;
        btnEditApacheConfig.Text = "Apache Config";
        // 
        // btnOpenApacheLog
        // 
        btnOpenApacheLog.Location = new Point(1013, 153);
        btnOpenApacheLog.Margin = new Padding(6);
        btnOpenApacheLog.Name = "btnOpenApacheLog";
        btnOpenApacheLog.Size = new Size(156, 60);
        btnOpenApacheLog.TabIndex = 15;
        btnOpenApacheLog.Text = "Apache Logs";
        // 
        // labelMySQL
        // 
        labelMySQL.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        labelMySQL.Location = new Point(47, 231);
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
        lblMySQLStatus.Location = new Point(326, 231);
        lblMySQLStatus.Margin = new Padding(6, 0, 6, 0);
        lblMySQLStatus.Name = "lblMySQLStatus";
        lblMySQLStatus.Size = new Size(175, 53);
        lblMySQLStatus.TabIndex = 6;
        lblMySQLStatus.Text = "Checking...";
        lblMySQLStatus.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // btnStartMySQL
        // 
        btnStartMySQL.Location = new Point(512, 227);
        btnStartMySQL.Margin = new Padding(6);
        btnStartMySQL.Name = "btnStartMySQL";
        btnStartMySQL.Size = new Size(121, 60);
        btnStartMySQL.TabIndex = 7;
        btnStartMySQL.Text = "Start";
        // 
        // btnStopMySQL
        // 
        btnStopMySQL.Location = new Point(644, 227);
        btnStopMySQL.Margin = new Padding(6);
        btnStopMySQL.Name = "btnStopMySQL";
        btnStopMySQL.Size = new Size(128, 60);
        btnStopMySQL.TabIndex = 8;
        btnStopMySQL.Text = "Stop";
        // 
        // btnEditMySQLConfig
        // 
        btnEditMySQLConfig.Location = new Point(783, 227);
        btnEditMySQLConfig.Margin = new Padding(6);
        btnEditMySQLConfig.Name = "btnEditMySQLConfig";
        btnEditMySQLConfig.Size = new Size(214, 60);
        btnEditMySQLConfig.TabIndex = 14;
        btnEditMySQLConfig.Text = "MySQL Config";
        // 
        // btnOpenMySQLLog
        // 
        btnOpenMySQLLog.Location = new Point(1013, 227);
        btnOpenMySQLLog.Margin = new Padding(6);
        btnOpenMySQLLog.Name = "btnOpenMySQLLog";
        btnOpenMySQLLog.Size = new Size(156, 60);
        btnOpenMySQLLog.TabIndex = 16;
        btnOpenMySQLLog.Text = "MySQL Logs";
        // 
        // labelLogHeader
        // 
        labelLogHeader.AutoSize = true;
        labelLogHeader.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        labelLogHeader.Location = new Point(47, 305);
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
        txtLog.Location = new Point(47, 352);
        txtLog.Margin = new Padding(6);
        txtLog.Multiline = true;
        txtLog.Name = "txtLog";
        txtLog.ReadOnly = true;
        txtLog.ScrollBars = ScrollBars.Vertical;
        txtLog.Size = new Size(1120, 306);
        txtLog.TabIndex = 10;
        // 
        // btnOpenWeb
        // 
        btnOpenWeb.Location = new Point(44, 677);
        btnOpenWeb.Margin = new Padding(6);
        btnOpenWeb.Name = "btnOpenWeb";
        btnOpenWeb.Size = new Size(223, 75);
        btnOpenWeb.TabIndex = 11;
        btnOpenWeb.Text = "Go to Website";
        // 
        // btnOpenPMA
        // 
        btnOpenPMA.Location = new Point(278, 677);
        btnOpenPMA.Margin = new Padding(6);
        btnOpenPMA.Name = "btnOpenPMA";
        btnOpenPMA.Size = new Size(280, 75);
        btnOpenPMA.TabIndex = 12;
        btnOpenPMA.Text = "phpMyAdmin";
        // 
        // btnEditPHPConfig
        // 
        btnEditPHPConfig.Location = new Point(931, 677);
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
        notifyIcon.Text = "Monrak Desktop Server";
        notifyIcon.Visible = true;
        // 
        // trayMenu
        // 
        trayMenu.ImageScalingSize = new Size(32, 32);
        trayMenu.Items.AddRange(new ToolStripItem[] { menuShow, aboutUsToolStripMenuItem, toolStripMenuItem1, menuExit });
        trayMenu.Name = "trayMenu";
        trayMenu.Size = new Size(340, 124);
        // 
        // menuShow
        // 
        menuShow.Name = "menuShow";
        menuShow.Size = new Size(339, 38);
        menuShow.Text = "Monrak Desktop Server";
        // 
        // aboutUsToolStripMenuItem
        // 
        aboutUsToolStripMenuItem.Name = "aboutUsToolStripMenuItem";
        aboutUsToolStripMenuItem.Size = new Size(339, 38);
        aboutUsToolStripMenuItem.Text = "About Us";
        aboutUsToolStripMenuItem.Click += aboutUsToolStripMenuItem_Click;
        // 
        // toolStripMenuItem1
        // 
        toolStripMenuItem1.Name = "toolStripMenuItem1";
        toolStripMenuItem1.Size = new Size(336, 6);
        // 
        // menuExit
        // 
        menuExit.Name = "menuExit";
        menuExit.Size = new Size(339, 38);
        menuExit.Text = "Exit";
        // 
        // chkStartWithWindows
        // 
        chkStartWithWindows.AutoSize = true;
        chkStartWithWindows.Checked = true;
        chkStartWithWindows.CheckState = CheckState.Checked;
        chkStartWithWindows.Location = new Point(931, 304);
        chkStartWithWindows.Margin = new Padding(6);
        chkStartWithWindows.Name = "chkStartWithWindows";
        chkStartWithWindows.Size = new Size(250, 36);
        chkStartWithWindows.TabIndex = 18;
        chkStartWithWindows.Text = "Start with Windows";
        chkStartWithWindows.UseVisualStyleBackColor = true;
        // 
        // btnEditPMAConfig
        // 
        btnEditPMAConfig.Location = new Point(569, 677);
        btnEditPMAConfig.Margin = new Padding(6);
        btnEditPMAConfig.Name = "btnEditPMAConfig";
        btnEditPMAConfig.Size = new Size(351, 75);
        btnEditPMAConfig.TabIndex = 19;
        btnEditPMAConfig.Text = "phpMyAdmin Config";
        // 
        // panel1
        // 
        panel1.BackColor = Color.Black;
        panel1.Controls.Add(pictureBox1);
        panel1.Dock = DockStyle.Top;
        panel1.Location = new Point(0, 0);
        panel1.Name = "panel1";
        panel1.Size = new Size(1205, 124);
        panel1.TabIndex = 20;
        // 
        // pictureBox1
        // 
        pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
        pictureBox1.Location = new Point(35, 3);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(598, 118);
        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        pictureBox1.TabIndex = 21;
        pictureBox1.TabStop = false;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1205, 782);
        Controls.Add(panel1);
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
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(6);
        MaximizeBox = false;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Monrak Desktop Server - ACTIVE";
        trayMenu.ResumeLayout(false);
        panel1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
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
    private ToolStripMenuItem menuExit;
    private ToolStripSeparator toolStripMenuItem1;
    private ToolStripMenuItem aboutUsToolStripMenuItem;
    private Panel panel1;
    private PictureBox pictureBox1;
}

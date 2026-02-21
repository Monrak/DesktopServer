namespace DesktopServerManagerGo;

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
        components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        notifyIcon = new NotifyIcon(components);
        trayMenu = new ContextMenuStrip(components);
        menuShow = new ToolStripMenuItem();
        menuAbout = new ToolStripMenuItem();
        toolStripMenuItem1 = new ToolStripSeparator();
        menuExit = new ToolStripMenuItem();
        pnlSidebar = new Panel();
        grpSSL = new GroupBox();
        btnTrustCert = new Button();
        chkEnableSSL = new CheckBox();
        chkRunOnStartup = new CheckBox();
        btnSettings = new Button();
        btnDashboard = new Button();
        lblSubtitle = new Label();
        btnLogs = new Button();
        lblTitle = new Label();
        btnMinimize = new Button();
        btnClose = new Button();
        timerMetrics = new System.Windows.Forms.Timer(components);
        pnlMain = new Panel();
        pnlLogs = new Panel();
        txtLogs = new RichTextBox();
        pnlServices = new Panel();
        pnlCards = new FlowLayoutPanel();
        cardRoadRunner = new Panel();
        btnStartRoadRunner = new Button();
        btnLogRoadRunner = new Button();
        lblRoadRunnerStatus = new Label();
        lblRoadRunnerTitle = new Label();
        cardMaria = new Panel();
        btnStartMaria = new Button();
        btnLogMaria = new Button();
        lblMariaStatus = new Label();
        lblMariaTitle = new Label();
        btnOpenPMA = new Button();
        cardPostgres = new Panel();
        btnStartPostgres = new Button();
        btnLogPostgres = new Button();
        lblPostgresStatus = new Label();
        lblPostgresTitle = new Label();
        btnOpenAdminer = new Button();
        cardPHP = new Panel();
        btnViewPhpIni = new Button();
        lblPHPStatus = new Label();
        lblPHPTitle = new Label();
        pnlDashboard = new Panel();
        lblRamValue = new Label();
        lblRamTitle = new Label();
        pnlRamChart = new Panel();
        lblCpuValue = new Label();
        lblCpuTitle = new Label();
        pnlCpuChart = new Panel();
        pnlHeader = new Panel();
        lblStatusHeader = new Label();
        trayMenu.SuspendLayout();
        pnlSidebar.SuspendLayout();
        grpSSL.SuspendLayout();
        pnlMain.SuspendLayout();
        pnlLogs.SuspendLayout();
        pnlServices.SuspendLayout();
        pnlCards.SuspendLayout();
        cardRoadRunner.SuspendLayout();
        cardMaria.SuspendLayout();
        cardPostgres.SuspendLayout();
        cardPHP.SuspendLayout();
        pnlDashboard.SuspendLayout();
        pnlHeader.SuspendLayout();
        SuspendLayout();
        // 
        // notifyIcon
        // 
        notifyIcon.ContextMenuStrip = trayMenu;
        notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
        notifyIcon.Text = "Monrak Desktop Server Go";
        notifyIcon.Visible = true;
        // 
        // trayMenu
        // 
        trayMenu.ImageScalingSize = new Size(32, 32);
        trayMenu.Items.AddRange(new ToolStripItem[] { menuShow, menuAbout, toolStripMenuItem1, menuExit });
        trayMenu.Name = "trayMenu";
        trayMenu.Size = new Size(317, 124);
        // 
        // menuShow
        // 
        menuShow.Name = "menuShow";
        menuShow.Size = new Size(316, 38);
        menuShow.Text = "Monrak Manager Go!";
        // 
        // menuAbout
        // 
        menuAbout.Name = "menuAbout";
        menuAbout.Size = new Size(316, 38);
        menuAbout.Text = "About Us";
        // 
        // toolStripMenuItem1
        // 
        toolStripMenuItem1.Name = "toolStripMenuItem1";
        toolStripMenuItem1.Size = new Size(313, 6);
        // 
        // menuExit
        // 
        menuExit.Name = "menuExit";
        menuExit.Size = new Size(316, 38);
        menuExit.Text = "Exit";
        // 
        // pnlSidebar
        // 
        pnlSidebar.BackColor = Color.FromArgb(30, 30, 46);
        pnlSidebar.Controls.Add(grpSSL);
        pnlSidebar.Controls.Add(btnSettings);
        pnlSidebar.Controls.Add(btnDashboard);
        pnlSidebar.Controls.Add(lblSubtitle);
        pnlSidebar.Controls.Add(btnLogs);
        pnlSidebar.Dock = DockStyle.Left;
        pnlSidebar.Location = new Point(0, 0);
        pnlSidebar.Margin = new Padding(6);
        pnlSidebar.Name = "pnlSidebar";
        pnlSidebar.Size = new Size(409, 1010);
        pnlSidebar.TabIndex = 0;
        // 
        // grpSSL
        // 
        grpSSL.Controls.Add(btnTrustCert);
        grpSSL.Controls.Add(chkEnableSSL);
        grpSSL.Controls.Add(chkRunOnStartup);
        grpSSL.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        grpSSL.ForeColor = Color.White;
        grpSSL.Location = new Point(10, 480);
        grpSSL.Name = "grpSSL";
        grpSSL.Size = new Size(385, 280);
        grpSSL.TabIndex = 4;
        grpSSL.TabStop = false;
        grpSSL.Text = "System Configuration";
        // 
        // btnTrustCert
        // 
        btnTrustCert.BackColor = Color.FromArgb(69, 71, 90);
        btnTrustCert.FlatAppearance.BorderSize = 0;
        btnTrustCert.FlatAppearance.MouseDownBackColor = Color.FromArgb(80, 82, 110);
        btnTrustCert.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 103, 130);
        btnTrustCert.FlatStyle = FlatStyle.Flat;
        btnTrustCert.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnTrustCert.ForeColor = Color.White;
        btnTrustCert.Location = new Point(20, 160);
        btnTrustCert.Name = "btnTrustCert";
        btnTrustCert.Size = new Size(345, 85);
        btnTrustCert.TabIndex = 1;
        btnTrustCert.Text = "TRUST CERTIFICATE";
        btnTrustCert.UseVisualStyleBackColor = false;
        // 
        // chkEnableSSL
        // 
        chkEnableSSL.AutoSize = true;
        chkEnableSSL.Font = new Font("Segoe UI", 10F);
        chkEnableSSL.ForeColor = Color.White;
        chkEnableSSL.Location = new Point(20, 95);
        chkEnableSSL.Name = "chkEnableSSL";
        chkEnableSSL.Size = new Size(246, 41);
        chkEnableSSL.TabIndex = 0;
        chkEnableSSL.Text = "Enable Local SSL";
        chkEnableSSL.UseVisualStyleBackColor = true;
        // 
        // chkRunOnStartup
        // 
        chkRunOnStartup.AutoSize = true;
        chkRunOnStartup.Font = new Font("Segoe UI", 10F);
        chkRunOnStartup.ForeColor = Color.White;
        chkRunOnStartup.Location = new Point(20, 48);
        chkRunOnStartup.Name = "chkRunOnStartup";
        chkRunOnStartup.Size = new Size(223, 41);
        chkRunOnStartup.TabIndex = 2;
        chkRunOnStartup.Text = "Run on startup";
        chkRunOnStartup.UseVisualStyleBackColor = true;
        // 
        // btnSettings
        // 
        btnSettings.FlatAppearance.BorderSize = 0;
        btnSettings.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 60);
        btnSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 50, 75);
        btnSettings.FlatStyle = FlatStyle.Flat;
        btnSettings.Font = new Font("Segoe UI", 11F);
        btnSettings.ForeColor = Color.White;
        btnSettings.Location = new Point(0, 232);
        btnSettings.Margin = new Padding(6);
        btnSettings.Name = "btnSettings";
        btnSettings.Size = new Size(409, 103);
        btnSettings.TabIndex = 2;
        btnSettings.Text = "  ⚙️ Settings";
        btnSettings.TextAlign = ContentAlignment.MiddleLeft;
        btnSettings.UseVisualStyleBackColor = false;
        // 
        // btnDashboard
        // 
        btnDashboard.FlatAppearance.BorderSize = 0;
        btnDashboard.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 60);
        btnDashboard.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 50, 75);
        btnDashboard.FlatStyle = FlatStyle.Flat;
        btnDashboard.Font = new Font("Segoe UI", 11F);
        btnDashboard.ForeColor = Color.White;
        btnDashboard.Location = new Point(0, 129);
        btnDashboard.Margin = new Padding(6);
        btnDashboard.Name = "btnDashboard";
        btnDashboard.Size = new Size(409, 103);
        btnDashboard.TabIndex = 1;
        btnDashboard.Text = "  📊 Dashboard";
        btnDashboard.TextAlign = ContentAlignment.MiddleLeft;
        btnDashboard.UseVisualStyleBackColor = false;
        // 
        // lblSubtitle
        // 
        lblSubtitle.BackColor = Color.Black;
        lblSubtitle.Dock = DockStyle.Top;
        lblSubtitle.Font = new Font("Segoe UI Black", 7.875F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
        lblSubtitle.ForeColor = Color.FromArgb(147, 153, 178);
        lblSubtitle.Location = new Point(0, 0);
        lblSubtitle.Margin = new Padding(6, 0, 6, 0);
        lblSubtitle.Name = "lblSubtitle";
        lblSubtitle.Size = new Size(409, 90);
        lblSubtitle.TabIndex = 5;
        lblSubtitle.Text = "Monrak Desktop Server Go!";
        lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // btnLogs
        // 
        btnLogs.FlatAppearance.BorderSize = 0;
        btnLogs.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 60);
        btnLogs.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 50, 75);
        btnLogs.FlatStyle = FlatStyle.Flat;
        btnLogs.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        btnLogs.ForeColor = Color.White;
        btnLogs.Location = new Point(0, 335);
        btnLogs.Name = "btnLogs";
        btnLogs.Padding = new Padding(20, 0, 0, 0);
        btnLogs.Size = new Size(406, 103);
        btnLogs.TabIndex = 3;
        btnLogs.Text = "📜Install Logs";
        btnLogs.TextAlign = ContentAlignment.MiddleLeft;
        btnLogs.UseVisualStyleBackColor = false;
        // 
        // lblTitle
        // 
        lblTitle.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(203, 166, 247);
        lblTitle.Location = new Point(0, 20);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(220, 35);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "MONRAK DESKTOP";
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // btnMinimize
        // 
        btnMinimize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnMinimize.Cursor = Cursors.Hand;
        btnMinimize.FlatAppearance.BorderSize = 0;
        btnMinimize.FlatStyle = FlatStyle.Flat;
        btnMinimize.Font = new Font("Segoe UI", 12F);
        btnMinimize.ForeColor = Color.White;
        btnMinimize.Location = new Point(1306, 21);
        btnMinimize.Margin = new Padding(6);
        btnMinimize.Name = "btnMinimize";
        btnMinimize.Size = new Size(56, 64);
        btnMinimize.TabIndex = 2;
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
        btnClose.Location = new Point(1374, 21);
        btnClose.Margin = new Padding(6);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(56, 64);
        btnClose.TabIndex = 1;
        btnClose.Text = "✕";
        btnClose.UseVisualStyleBackColor = true;
        // 
        // pnlMain
        // 
        pnlMain.BackColor = Color.FromArgb(17, 17, 27);
        pnlMain.Controls.Add(pnlLogs);
        pnlMain.Controls.Add(pnlServices);
        pnlMain.Controls.Add(pnlDashboard);
        pnlMain.Controls.Add(pnlHeader);
        pnlMain.Dock = DockStyle.Fill;
        pnlMain.Location = new Point(409, 0);
        pnlMain.Margin = new Padding(6);
        pnlMain.Name = "pnlMain";
        pnlMain.Size = new Size(1448, 1010);
        pnlMain.TabIndex = 1;
        // 
        // pnlLogs
        // 
        pnlLogs.BackColor = Color.FromArgb(17, 17, 27);
        pnlLogs.Controls.Add(txtLogs);
        pnlLogs.Dock = DockStyle.Fill;
        pnlLogs.Location = new Point(0, 130);
        pnlLogs.Name = "pnlLogs";
        pnlLogs.Padding = new Padding(20);
        pnlLogs.Size = new Size(1448, 880);
        pnlLogs.TabIndex = 2;
        pnlLogs.Visible = false;
        // 
        // txtLogs
        // 
        txtLogs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        txtLogs.BackColor = Color.FromArgb(30, 30, 46);
        txtLogs.BorderStyle = BorderStyle.None;
        txtLogs.Font = new Font("Consolas", 10F);
        txtLogs.ForeColor = Color.FromArgb(166, 227, 161);
        txtLogs.Location = new Point(20, 20);
        txtLogs.Name = "txtLogs";
        txtLogs.ReadOnly = true;
        txtLogs.Size = new Size(1408, 840);
        txtLogs.TabIndex = 0;
        txtLogs.Text = "";
        // 
        // pnlServices
        // 
        pnlServices.Controls.Add(pnlCards);
        pnlServices.Dock = DockStyle.Fill;
        pnlServices.Location = new Point(0, 130);
        pnlServices.Name = "pnlServices";
        pnlServices.Padding = new Padding(37, 0, 37, 43);
        pnlServices.Size = new Size(1448, 880);
        pnlServices.TabIndex = 2;
        // 
        // pnlCards
        // 
        pnlCards.Controls.Add(cardRoadRunner);
        pnlCards.Controls.Add(cardMaria);
        pnlCards.Controls.Add(cardPostgres);
        pnlCards.Controls.Add(cardPHP);
        pnlCards.Dock = DockStyle.Fill;
        pnlCards.Location = new Point(37, 0);
        pnlCards.Margin = new Padding(6);
        pnlCards.Name = "pnlCards";
        pnlCards.Size = new Size(1374, 837);
        pnlCards.TabIndex = 1;
        // 
        // cardRoadRunner
        // 
        cardRoadRunner.BackColor = Color.FromArgb(30, 30, 46);
        cardRoadRunner.Controls.Add(btnStartRoadRunner);
        cardRoadRunner.Controls.Add(btnLogRoadRunner);
        cardRoadRunner.Controls.Add(lblRoadRunnerStatus);
        cardRoadRunner.Controls.Add(lblRoadRunnerTitle);
        cardRoadRunner.Location = new Point(19, 21);
        cardRoadRunner.Margin = new Padding(19, 21, 19, 21);
        cardRoadRunner.Name = "cardRoadRunner";
        cardRoadRunner.Size = new Size(409, 384);
        cardRoadRunner.TabIndex = 0;
        // 
        // btnStartRoadRunner
        // 
        btnStartRoadRunner.BackColor = Color.FromArgb(137, 180, 250);
        btnStartRoadRunner.FlatAppearance.BorderSize = 0;
        btnStartRoadRunner.FlatAppearance.MouseDownBackColor = Color.FromArgb(117, 160, 230);
        btnStartRoadRunner.FlatAppearance.MouseOverBackColor = Color.FromArgb(157, 200, 255);
        btnStartRoadRunner.FlatStyle = FlatStyle.Flat;
        btnStartRoadRunner.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        btnStartRoadRunner.ForeColor = Color.FromArgb(17, 17, 27);
        btnStartRoadRunner.Location = new Point(28, 267);
        btnStartRoadRunner.Margin = new Padding(6);
        btnStartRoadRunner.Name = "btnStartRoadRunner";
        btnStartRoadRunner.Size = new Size(353, 85);
        btnStartRoadRunner.TabIndex = 2;
        btnStartRoadRunner.Text = "START";
        btnStartRoadRunner.UseVisualStyleBackColor = false;
        // 
        // btnLogRoadRunner
        // 
        btnLogRoadRunner.BackColor = Color.FromArgb(69, 71, 90);
        btnLogRoadRunner.FlatAppearance.BorderSize = 0;
        btnLogRoadRunner.FlatAppearance.MouseDownBackColor = Color.FromArgb(80, 82, 110);
        btnLogRoadRunner.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 103, 130);
        btnLogRoadRunner.FlatStyle = FlatStyle.Flat;
        btnLogRoadRunner.Font = new Font("Segoe UI", 9F);
        btnLogRoadRunner.ForeColor = Color.White;
        btnLogRoadRunner.Location = new Point(28, 200);
        btnLogRoadRunner.Name = "btnLogRoadRunner";
        btnLogRoadRunner.Size = new Size(353, 50);
        btnLogRoadRunner.TabIndex = 4;
        btnLogRoadRunner.Text = "VIEW LOG";
        btnLogRoadRunner.UseVisualStyleBackColor = false;
        // 
        // lblRoadRunnerStatus
        // 
        lblRoadRunnerStatus.AutoSize = true;
        lblRoadRunnerStatus.Font = new Font("Segoe UI", 10F);
        lblRoadRunnerStatus.ForeColor = Color.FromArgb(243, 139, 168);
        lblRoadRunnerStatus.Location = new Point(28, 96);
        lblRoadRunnerStatus.Margin = new Padding(6, 0, 6, 0);
        lblRoadRunnerStatus.Name = "lblRoadRunnerStatus";
        lblRoadRunnerStatus.Size = new Size(117, 37);
        lblRoadRunnerStatus.TabIndex = 1;
        lblRoadRunnerStatus.Text = "Stopped";
        // 
        // lblRoadRunnerTitle
        // 
        lblRoadRunnerTitle.AutoSize = true;
        lblRoadRunnerTitle.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        lblRoadRunnerTitle.ForeColor = Color.FromArgb(137, 180, 250);
        lblRoadRunnerTitle.Location = new Point(28, 32);
        lblRoadRunnerTitle.Margin = new Padding(6, 0, 6, 0);
        lblRoadRunnerTitle.Name = "lblRoadRunnerTitle";
        lblRoadRunnerTitle.Size = new Size(254, 45);
        lblRoadRunnerTitle.TabIndex = 0;
        lblRoadRunnerTitle.Text = "RoadRunner 🏎️";
        // 
        // cardMaria
        // 
        cardMaria.BackColor = Color.FromArgb(30, 30, 46);
        cardMaria.Controls.Add(btnStartMaria);
        cardMaria.Controls.Add(btnLogMaria);
        cardMaria.Controls.Add(lblMariaStatus);
        cardMaria.Controls.Add(lblMariaTitle);
        cardMaria.Controls.Add(btnOpenPMA);
        cardMaria.Location = new Point(466, 21);
        cardMaria.Margin = new Padding(19, 21, 19, 21);
        cardMaria.Name = "cardMaria";
        cardMaria.Size = new Size(409, 384);
        cardMaria.TabIndex = 1;
        // 
        // btnStartMaria
        // 
        btnStartMaria.BackColor = Color.FromArgb(166, 227, 161);
        btnStartMaria.FlatAppearance.BorderSize = 0;
        btnStartMaria.FlatAppearance.MouseDownBackColor = Color.FromArgb(146, 207, 141);
        btnStartMaria.FlatAppearance.MouseOverBackColor = Color.FromArgb(186, 247, 181);
        btnStartMaria.FlatStyle = FlatStyle.Flat;
        btnStartMaria.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        btnStartMaria.ForeColor = Color.FromArgb(17, 17, 27);
        btnStartMaria.Location = new Point(28, 267);
        btnStartMaria.Margin = new Padding(6);
        btnStartMaria.Name = "btnStartMaria";
        btnStartMaria.Size = new Size(353, 85);
        btnStartMaria.TabIndex = 2;
        btnStartMaria.Text = "START";
        btnStartMaria.UseVisualStyleBackColor = false;
        // 
        // btnLogMaria
        // 
        btnLogMaria.BackColor = Color.FromArgb(69, 71, 90);
        btnLogMaria.FlatAppearance.BorderSize = 0;
        btnLogMaria.FlatAppearance.MouseDownBackColor = Color.FromArgb(80, 82, 110);
        btnLogMaria.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 103, 130);
        btnLogMaria.FlatStyle = FlatStyle.Flat;
        btnLogMaria.Font = new Font("Segoe UI", 9F);
        btnLogMaria.ForeColor = Color.White;
        btnLogMaria.Location = new Point(28, 200);
        btnLogMaria.Name = "btnLogMaria";
        btnLogMaria.Size = new Size(353, 50);
        btnLogMaria.TabIndex = 4;
        btnLogMaria.Text = "VIEW LOG";
        btnLogMaria.UseVisualStyleBackColor = false;
        // 
        // lblMariaStatus
        // 
        lblMariaStatus.AutoSize = true;
        lblMariaStatus.Font = new Font("Segoe UI", 10F);
        lblMariaStatus.ForeColor = Color.FromArgb(243, 139, 168);
        lblMariaStatus.Location = new Point(28, 96);
        lblMariaStatus.Margin = new Padding(6, 0, 6, 0);
        lblMariaStatus.Name = "lblMariaStatus";
        lblMariaStatus.Size = new Size(117, 37);
        lblMariaStatus.TabIndex = 1;
        lblMariaStatus.Text = "Stopped";
        // 
        // lblMariaTitle
        // 
        lblMariaTitle.AutoSize = true;
        lblMariaTitle.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        lblMariaTitle.ForeColor = Color.FromArgb(166, 227, 161);
        lblMariaTitle.Location = new Point(28, 32);
        lblMariaTitle.Margin = new Padding(6, 0, 6, 0);
        lblMariaTitle.Name = "lblMariaTitle";
        lblMariaTitle.Size = new Size(200, 45);
        lblMariaTitle.TabIndex = 0;
        lblMariaTitle.Text = "MariaDB \U0001f9ad";
        // 
        // btnOpenPMA
        // 
        btnOpenPMA.BackColor = Color.FromArgb(69, 71, 90);
        btnOpenPMA.FlatAppearance.BorderSize = 0;
        btnOpenPMA.FlatAppearance.MouseDownBackColor = Color.FromArgb(80, 82, 110);
        btnOpenPMA.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 103, 130);
        btnOpenPMA.FlatStyle = FlatStyle.Flat;
        btnOpenPMA.Font = new Font("Segoe UI", 9F);
        btnOpenPMA.ForeColor = Color.White;
        btnOpenPMA.Location = new Point(28, 140);
        btnOpenPMA.Name = "btnOpenPMA";
        btnOpenPMA.Size = new Size(353, 50);
        btnOpenPMA.TabIndex = 3;
        btnOpenPMA.Text = "OPEN PHPMYADMIN";
        btnOpenPMA.UseVisualStyleBackColor = false;
        // 
        // cardPostgres
        // 
        cardPostgres.BackColor = Color.FromArgb(30, 30, 46);
        cardPostgres.Controls.Add(btnStartPostgres);
        cardPostgres.Controls.Add(btnLogPostgres);
        cardPostgres.Controls.Add(lblPostgresStatus);
        cardPostgres.Controls.Add(lblPostgresTitle);
        cardPostgres.Controls.Add(btnOpenAdminer);
        cardPostgres.Location = new Point(913, 21);
        cardPostgres.Margin = new Padding(19, 21, 19, 21);
        cardPostgres.Name = "cardPostgres";
        cardPostgres.Size = new Size(409, 384);
        cardPostgres.TabIndex = 2;
        // 
        // btnStartPostgres
        // 
        btnStartPostgres.BackColor = Color.FromArgb(203, 166, 247);
        btnStartPostgres.FlatAppearance.BorderSize = 0;
        btnStartPostgres.FlatAppearance.MouseDownBackColor = Color.FromArgb(183, 146, 227);
        btnStartPostgres.FlatAppearance.MouseOverBackColor = Color.FromArgb(223, 186, 255);
        btnStartPostgres.FlatStyle = FlatStyle.Flat;
        btnStartPostgres.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        btnStartPostgres.ForeColor = Color.FromArgb(17, 17, 27);
        btnStartPostgres.Location = new Point(28, 267);
        btnStartPostgres.Margin = new Padding(6);
        btnStartPostgres.Name = "btnStartPostgres";
        btnStartPostgres.Size = new Size(353, 85);
        btnStartPostgres.TabIndex = 2;
        btnStartPostgres.Text = "START";
        btnStartPostgres.UseVisualStyleBackColor = false;
        // 
        // btnLogPostgres
        // 
        btnLogPostgres.BackColor = Color.FromArgb(69, 71, 90);
        btnLogPostgres.FlatAppearance.BorderSize = 0;
        btnLogPostgres.FlatAppearance.MouseDownBackColor = Color.FromArgb(80, 82, 110);
        btnLogPostgres.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 103, 130);
        btnLogPostgres.FlatStyle = FlatStyle.Flat;
        btnLogPostgres.Font = new Font("Segoe UI", 9F);
        btnLogPostgres.ForeColor = Color.White;
        btnLogPostgres.Location = new Point(28, 200);
        btnLogPostgres.Name = "btnLogPostgres";
        btnLogPostgres.Size = new Size(353, 50);
        btnLogPostgres.TabIndex = 4;
        btnLogPostgres.Text = "VIEW LOG";
        btnLogPostgres.UseVisualStyleBackColor = false;
        // 
        // lblPostgresStatus
        // 
        lblPostgresStatus.AutoSize = true;
        lblPostgresStatus.Font = new Font("Segoe UI", 10F);
        lblPostgresStatus.ForeColor = Color.FromArgb(243, 139, 168);
        lblPostgresStatus.Location = new Point(28, 96);
        lblPostgresStatus.Margin = new Padding(6, 0, 6, 0);
        lblPostgresStatus.Name = "lblPostgresStatus";
        lblPostgresStatus.Size = new Size(117, 37);
        lblPostgresStatus.TabIndex = 1;
        lblPostgresStatus.Text = "Stopped";
        // 
        // lblPostgresTitle
        // 
        lblPostgresTitle.AutoSize = true;
        lblPostgresTitle.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        lblPostgresTitle.ForeColor = Color.FromArgb(203, 166, 247);
        lblPostgresTitle.Location = new Point(28, 32);
        lblPostgresTitle.Margin = new Padding(6, 0, 6, 0);
        lblPostgresTitle.Name = "lblPostgresTitle";
        lblPostgresTitle.Size = new Size(242, 45);
        lblPostgresTitle.TabIndex = 0;
        lblPostgresTitle.Text = "PostgreSQL 🐘";
        // 
        // btnOpenAdminer
        // 
        btnOpenAdminer.BackColor = Color.FromArgb(69, 71, 90);
        btnOpenAdminer.FlatAppearance.BorderSize = 0;
        btnOpenAdminer.FlatAppearance.MouseDownBackColor = Color.FromArgb(80, 82, 110);
        btnOpenAdminer.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 103, 130);
        btnOpenAdminer.FlatStyle = FlatStyle.Flat;
        btnOpenAdminer.Font = new Font("Segoe UI", 9F);
        btnOpenAdminer.ForeColor = Color.White;
        btnOpenAdminer.Location = new Point(28, 140);
        btnOpenAdminer.Name = "btnOpenAdminer";
        btnOpenAdminer.Size = new Size(353, 50);
        btnOpenAdminer.TabIndex = 3;
        btnOpenAdminer.Text = "OPEN ADMINER";
        btnOpenAdminer.UseVisualStyleBackColor = false;
        // 
        // cardPHP
        // 
        cardPHP.BackColor = Color.FromArgb(30, 30, 46);
        cardPHP.Controls.Add(btnViewPhpIni);
        cardPHP.Controls.Add(lblPHPStatus);
        cardPHP.Controls.Add(lblPHPTitle);
        cardPHP.Location = new Point(19, 447);
        cardPHP.Margin = new Padding(19, 21, 19, 21);
        cardPHP.Name = "cardPHP";
        cardPHP.Size = new Size(409, 384);
        cardPHP.TabIndex = 3;
        // 
        // btnViewPhpIni
        // 
        btnViewPhpIni.BackColor = Color.FromArgb(249, 226, 175);
        btnViewPhpIni.FlatAppearance.BorderSize = 0;
        btnViewPhpIni.FlatAppearance.MouseDownBackColor = Color.FromArgb(249, 216, 155);
        btnViewPhpIni.FlatAppearance.MouseOverBackColor = Color.FromArgb(250, 236, 205);
        btnViewPhpIni.FlatStyle = FlatStyle.Flat;
        btnViewPhpIni.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        btnViewPhpIni.ForeColor = Color.FromArgb(17, 17, 27);
        btnViewPhpIni.Location = new Point(28, 267);
        btnViewPhpIni.Margin = new Padding(6);
        btnViewPhpIni.Name = "btnViewPhpIni";
        btnViewPhpIni.Size = new Size(353, 85);
        btnViewPhpIni.TabIndex = 2;
        btnViewPhpIni.Text = "VIEW PHP.INI";
        btnViewPhpIni.UseVisualStyleBackColor = false;
        // 
        // lblPHPStatus
        // 
        lblPHPStatus.AutoSize = true;
        lblPHPStatus.Font = new Font("Segoe UI", 10F);
        lblPHPStatus.ForeColor = Color.FromArgb(249, 226, 175);
        lblPHPStatus.Location = new Point(28, 96);
        lblPHPStatus.Margin = new Padding(6, 0, 6, 0);
        lblPHPStatus.Name = "lblPHPStatus";
        lblPHPStatus.Size = new Size(181, 37);
        lblPHPStatus.TabIndex = 1;
        lblPHPStatus.Text = "Configuration";
        // 
        // lblPHPTitle
        // 
        lblPHPTitle.AutoSize = true;
        lblPHPTitle.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        lblPHPTitle.ForeColor = Color.FromArgb(249, 226, 175);
        lblPHPTitle.Location = new Point(28, 32);
        lblPHPTitle.Margin = new Padding(6, 0, 6, 0);
        lblPHPTitle.Name = "lblPHPTitle";
        lblPHPTitle.Size = new Size(189, 45);
        lblPHPTitle.TabIndex = 0;
        lblPHPTitle.Text = "PHP 8.4 🐘";
        // 
        // pnlDashboard
        // 
        pnlDashboard.Controls.Add(lblRamValue);
        pnlDashboard.Controls.Add(lblRamTitle);
        pnlDashboard.Controls.Add(pnlRamChart);
        pnlDashboard.Controls.Add(lblCpuValue);
        pnlDashboard.Controls.Add(lblCpuTitle);
        pnlDashboard.Controls.Add(pnlCpuChart);
        pnlDashboard.Dock = DockStyle.Fill;
        pnlDashboard.Location = new Point(0, 130);
        pnlDashboard.Name = "pnlDashboard";
        pnlDashboard.Padding = new Padding(37, 20, 37, 43);
        pnlDashboard.Size = new Size(1448, 880);
        pnlDashboard.TabIndex = 3;
        pnlDashboard.Visible = false;
        // 
        // lblRamValue
        // 
        lblRamValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblRamValue.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblRamValue.ForeColor = Color.White;
        lblRamValue.Location = new Point(1011, 440);
        lblRamValue.Name = "lblRamValue";
        lblRamValue.Size = new Size(400, 45);
        lblRamValue.TabIndex = 0;
        lblRamValue.Text = "0 MB";
        lblRamValue.TextAlign = ContentAlignment.TopRight;
        // 
        // lblRamTitle
        // 
        lblRamTitle.AutoSize = true;
        lblRamTitle.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        lblRamTitle.ForeColor = Color.FromArgb(166, 227, 161);
        lblRamTitle.Location = new Point(74, 440);
        lblRamTitle.Name = "lblRamTitle";
        lblRamTitle.Size = new Size(200, 45);
        lblRamTitle.TabIndex = 1;
        lblRamTitle.Text = "RAM USAGE";
        // 
        // pnlRamChart
        // 
        pnlRamChart.BackColor = Color.FromArgb(30, 30, 46);
        pnlRamChart.Location = new Point(37, 480);
        pnlRamChart.Name = "pnlRamChart";
        pnlRamChart.Size = new Size(1374, 300);
        pnlRamChart.TabIndex = 1;
        // 
        // lblCpuValue
        // 
        lblCpuValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblCpuValue.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblCpuValue.ForeColor = Color.White;
        lblCpuValue.Location = new Point(1211, 40);
        lblCpuValue.Name = "lblCpuValue";
        lblCpuValue.Size = new Size(200, 45);
        lblCpuValue.TabIndex = 2;
        lblCpuValue.Text = "0%";
        lblCpuValue.TextAlign = ContentAlignment.TopRight;
        // 
        // lblCpuTitle
        // 
        lblCpuTitle.AutoSize = true;
        lblCpuTitle.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        lblCpuTitle.ForeColor = Color.FromArgb(137, 180, 250);
        lblCpuTitle.Location = new Point(74, 40);
        lblCpuTitle.Name = "lblCpuTitle";
        lblCpuTitle.Size = new Size(191, 45);
        lblCpuTitle.TabIndex = 3;
        lblCpuTitle.Text = "CPU USAGE";
        // 
        // pnlCpuChart
        // 
        pnlCpuChart.BackColor = Color.FromArgb(30, 30, 46);
        pnlCpuChart.Location = new Point(37, 80);
        pnlCpuChart.Name = "pnlCpuChart";
        pnlCpuChart.Size = new Size(1374, 300);
        pnlCpuChart.TabIndex = 0;
        // 
        // pnlHeader
        // 
        pnlHeader.Controls.Add(lblStatusHeader);
        pnlHeader.Controls.Add(btnMinimize);
        pnlHeader.Controls.Add(btnClose);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Margin = new Padding(6);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(1448, 130);
        pnlHeader.TabIndex = 0;
        // 
        // lblStatusHeader
        // 
        lblStatusHeader.AutoSize = true;
        lblStatusHeader.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
        lblStatusHeader.ForeColor = Color.White;
        lblStatusHeader.Location = new Point(19, 31);
        lblStatusHeader.Margin = new Padding(6, 0, 6, 0);
        lblStatusHeader.Name = "lblStatusHeader";
        lblStatusHeader.Size = new Size(363, 65);
        lblStatusHeader.TabIndex = 0;
        lblStatusHeader.Text = "Service Control";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(17, 17, 27);
        ClientSize = new Size(1857, 1010);
        Controls.Add(pnlMain);
        Controls.Add(pnlSidebar);
        FormBorderStyle = FormBorderStyle.None;
        Margin = new Padding(6);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "DesktopServer Go";
        trayMenu.ResumeLayout(false);
        pnlSidebar.ResumeLayout(false);
        grpSSL.ResumeLayout(false);
        grpSSL.PerformLayout();
        pnlMain.ResumeLayout(false);
        pnlLogs.ResumeLayout(false);
        pnlServices.ResumeLayout(false);
        pnlCards.ResumeLayout(false);
        cardRoadRunner.ResumeLayout(false);
        cardRoadRunner.PerformLayout();
        cardMaria.ResumeLayout(false);
        cardMaria.PerformLayout();
        cardPostgres.ResumeLayout(false);
        cardPostgres.PerformLayout();
        cardPHP.ResumeLayout(false);
        cardPHP.PerformLayout();
        pnlDashboard.ResumeLayout(false);
        pnlDashboard.PerformLayout();
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ResumeLayout(false);
    }

    private Panel pnlSidebar;
    private Label lblTitle;
    private Label lblSubtitle;
    private Button btnDashboard;
    private Button btnSettings;
    private Button btnLogs;
    private Panel pnlMain;
    private Panel pnlServices;
    private Panel pnlDashboard;
    private Label lblCpuTitle;
    private Label lblCpuValue;
    private Panel pnlCpuChart;
    private Label lblRamTitle;
    private Label lblRamValue;
    private Panel pnlRamChart;
    private System.Windows.Forms.Timer timerMetrics;
    private Panel pnlHeader;
    private Label lblStatusHeader;
    private FlowLayoutPanel pnlCards;
    private Panel cardRoadRunner;
    private Label lblRoadRunnerTitle;
    private Label lblRoadRunnerStatus;
    private Button btnStartRoadRunner;
    private Panel cardMaria;
    private Label lblMariaTitle;
    private Label lblMariaStatus;
    private Button btnStartMaria;
    private Panel cardPostgres;
    private Label lblPostgresTitle;
    private Label lblPostgresStatus;
    private Button btnStartPostgres;
    private Button btnOpenPMA;
    private Button btnOpenAdminer;
    private Button btnMinimize;
    private Button btnClose;
    private CheckBox chkEnableSSL;
    private CheckBox chkRunOnStartup;
    private Button btnTrustCert;
    private NotifyIcon notifyIcon;
    private ContextMenuStrip trayMenu;
    private ToolStripMenuItem menuShow;
    private ToolStripMenuItem menuAbout;
    private ToolStripSeparator toolStripMenuItem1;
    private ToolStripMenuItem menuExit;
    private Panel pnlLogs;
    private RichTextBox txtLogs;
    private Panel cardPHP;
    private Label lblPHPTitle;
    private Label lblPHPStatus;
    private Button btnViewPhpIni;
    private Button btnLogRoadRunner;
    private Button btnLogMaria;
    private Button btnLogPostgres;
    private GroupBox grpSSL;
}

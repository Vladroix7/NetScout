using System.Drawing;
using System.Windows.Forms;

namespace NetScoutServer
{
    partial class AdminDashboard
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // ── Controls ──────────────────────────────────────────────
            panelHeader       = new Panel();
            lblTitle          = new Label();
            lblStatus         = new Label();

            splitMain         = new SplitContainer();   // Left | Right
            splitRight        = new SplitContainer();   // Top (grid) | Bottom (log)

            panelOnline       = new Panel();
            lblOnlineTitle    = new Label();
            lblOnlineCount    = new Label();
            lstOnline         = new ListBox();
            btnClearOnline    = new Button();
            lblPwdTitle       = new Label();
            lblPwdCount       = new Label();
            lstPwdRequests    = new ListBox();
            btnRespondPwd     = new Button();

            dgvUsers          = new DataGridView();
            panelActions      = new Panel();
            lblUserCount      = new Label();
            btnRefresh        = new Button();
            btnDelete         = new Button();
            btnToggleAdmin    = new Button();
            btnChangePassword = new Button();

            rtbLog            = new RichTextBox();

            // ── Header ────────────────────────────────────────────────
            panelHeader.Dock      = DockStyle.Top;
            panelHeader.Height    = 58;
            panelHeader.BackColor = Color.DarkSlateGray;
            panelHeader.Padding   = new Padding(12, 0, 12, 0);

            lblTitle.Text      = "NetScout  —  Admin Server v1.0";
            lblTitle.Font      = new Font("Segoe UI", 15, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize  = true;
            lblTitle.Location  = new Point(12, 14);

            lblStatus.Text      = "● INITIALIZING";
            lblStatus.Font      = new Font("Segoe UI", 10, FontStyle.Bold);
            lblStatus.ForeColor = Color.Yellow;
            lblStatus.AutoSize  = true;
            lblStatus.Anchor    = AnchorStyles.Top | AnchorStyles.Right;
            lblStatus.Location  = new Point(820, 20);

            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(lblStatus);

            // ── Split: Left (online users) | Right (grid + log) ──────
            splitMain.Dock          = DockStyle.Fill;
            splitMain.Orientation   = Orientation.Vertical;
            splitMain.FixedPanel    = FixedPanel.Panel1;
            splitMain.SplitterWidth = 2;

            // ── Online Users Panel (left) ─────────────────────────────
            panelOnline.Dock      = DockStyle.Fill;
            panelOnline.BackColor = Color.FromArgb(245, 245, 245);
            panelOnline.Padding   = new Padding(6);

            lblOnlineTitle.Text      = "RECENTLY ONLINE";
            lblOnlineTitle.Font      = new Font("Segoe UI", 8, FontStyle.Bold);
            lblOnlineTitle.ForeColor = Color.DarkSlateGray;
            lblOnlineTitle.AutoSize  = true;
            lblOnlineTitle.Location  = new Point(6, 8);

            lblOnlineCount.Text      = "Online: 0";
            lblOnlineCount.Font      = new Font("Segoe UI", 8);
            lblOnlineCount.ForeColor = Color.Gray;
            lblOnlineCount.AutoSize  = true;
            lblOnlineCount.Location  = new Point(6, 26);

            lstOnline.Location    = new Point(6, 46);
            lstOnline.Size        = new Size(196, 170);
            lstOnline.Font        = new Font("Segoe UI", 8);
            lstOnline.BorderStyle = BorderStyle.FixedSingle;
            lstOnline.Anchor      = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            btnClearOnline.Text      = "Clear";
            btnClearOnline.Font      = new Font("Segoe UI", 8);
            btnClearOnline.FlatStyle = FlatStyle.Flat;
            btnClearOnline.BackColor = Color.DarkSlateGray;
            btnClearOnline.ForeColor = Color.White;
            btnClearOnline.Size      = new Size(60, 22);
            btnClearOnline.Anchor    = AnchorStyles.Top | AnchorStyles.Left;
            btnClearOnline.Location  = new Point(6, 222);
            btnClearOnline.Click    += btnClearOnline_Click;

            // lblPwdTitle
            lblPwdTitle.Text      = "PWD RESET REQUESTS";
            lblPwdTitle.Font      = new Font("Segoe UI", 8, FontStyle.Bold);
            lblPwdTitle.ForeColor = Color.Crimson;
            lblPwdTitle.AutoSize  = true;
            lblPwdTitle.Location  = new Point(6, 256);

            // lblPwdCount
            lblPwdCount.Text      = "Pending: 0";
            lblPwdCount.Font      = new Font("Segoe UI", 8);
            lblPwdCount.ForeColor = Color.Gray;
            lblPwdCount.AutoSize  = true;
            lblPwdCount.Location  = new Point(6, 274);

            // lstPwdRequests
            lstPwdRequests.Location    = new Point(6, 292);
            lstPwdRequests.Size        = new Size(196, 150);
            lstPwdRequests.Font        = new Font("Segoe UI", 8);
            lstPwdRequests.BorderStyle = BorderStyle.FixedSingle;
            lstPwdRequests.Anchor      = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // btnRespondPwd
            btnRespondPwd.Text      = "🔑 Reset Password";
            btnRespondPwd.Font      = new Font("Segoe UI", 8, FontStyle.Bold);
            btnRespondPwd.FlatStyle = FlatStyle.Flat;
            btnRespondPwd.BackColor = Color.Crimson;
            btnRespondPwd.ForeColor = Color.White;
            btnRespondPwd.FlatAppearance.BorderSize = 0;
            btnRespondPwd.Size      = new Size(196, 26);
            btnRespondPwd.Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnRespondPwd.Location  = new Point(6, 448);
            btnRespondPwd.Cursor    = Cursors.Hand;
            btnRespondPwd.Click    += btnRespondPwd_Click;

            panelOnline.Controls.Add(lblOnlineTitle);
            panelOnline.Controls.Add(lblOnlineCount);
            panelOnline.Controls.Add(lstOnline);
            panelOnline.Controls.Add(btnClearOnline);
            panelOnline.Controls.Add(lblPwdTitle);
            panelOnline.Controls.Add(lblPwdCount);
            panelOnline.Controls.Add(lstPwdRequests);
            panelOnline.Controls.Add(btnRespondPwd);
            splitMain.Panel1.Controls.Add(panelOnline);

            // ── Split Right: Users grid (top) | Log (bottom) ──────────
            splitRight.Dock          = DockStyle.Fill;
            splitRight.Orientation   = Orientation.Horizontal;
            splitRight.SplitterWidth = 2;

            // ── DataGridView ───────────────────────────────────────────
            SetupDataGridView();
            splitRight.Panel1.Controls.Add(dgvUsers);
            splitRight.Panel1.Controls.Add(panelActions);

            // ── Action Buttons ─────────────────────────────────────────
            panelActions.Dock      = DockStyle.Bottom;
            panelActions.Height    = 48;
            panelActions.BackColor = Color.FromArgb(238, 238, 238);
            panelActions.Padding   = new Padding(6, 8, 6, 6);

            lblUserCount.Text      = "Total Users: 0";
            lblUserCount.Font      = new Font("Segoe UI", 9);
            lblUserCount.ForeColor = Color.DarkSlateGray;
            lblUserCount.AutoSize  = true;
            lblUserCount.Location  = new Point(8, 15);

            StyleButton(btnRefresh,        "⟳  Refresh",         Color.SteelBlue,     90,  6);
            StyleButton(btnDelete,         "🗑  Delete User",     Color.Crimson,      222,  6);
            StyleButton(btnToggleAdmin,    "★  Toggle Admin",     Color.DarkSlateGray, 354,  6);
            StyleButton(btnChangePassword, "🔑  Change Password", Color.DimGray,       486,  6);

            btnRefresh.Click        += btnRefresh_Click;
            btnDelete.Click         += btnDelete_Click;
            btnToggleAdmin.Click    += btnToggleAdmin_Click;
            btnChangePassword.Click += btnChangePassword_Click;

            panelActions.Controls.Add(lblUserCount);
            panelActions.Controls.Add(btnRefresh);
            panelActions.Controls.Add(btnDelete);
            panelActions.Controls.Add(btnToggleAdmin);
            panelActions.Controls.Add(btnChangePassword);

            // ── Log ────────────────────────────────────────────────────
            rtbLog.Dock        = DockStyle.Fill;
            rtbLog.BackColor   = Color.FromArgb(20, 20, 20);
            rtbLog.ForeColor   = Color.LightGreen;
            rtbLog.Font        = new Font("Consolas", 9);
            rtbLog.ReadOnly    = true;
            rtbLog.BorderStyle = BorderStyle.None;
            rtbLog.ScrollBars  = RichTextBoxScrollBars.Vertical;
            splitRight.Panel2.Controls.Add(rtbLog);

            splitMain.Panel2.Controls.Add(splitRight);

            // ── Form ───────────────────────────────────────────────────
            this.Text            = "NetScout Admin";
            this.Size            = new Size(1050, 700);
            this.MinimumSize     = new Size(900, 580);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.BackColor       = Color.WhiteSmoke;
            this.Load           += AdminDashboard_Load;
            this.FormClosing    += AdminDashboard_FormClosing;

            this.Controls.Add(splitMain);
            this.Controls.Add(panelHeader);
        }

        private void SetupDataGridView()
        {
            dgvUsers.Dock                    = DockStyle.Fill;
            dgvUsers.ReadOnly                = true;
            dgvUsers.SelectionMode           = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.MultiSelect             = false;
            dgvUsers.AllowUserToAddRows      = false;
            dgvUsers.AllowUserToDeleteRows   = false;
            dgvUsers.AllowUserToResizeRows   = false;
            dgvUsers.RowHeadersVisible       = false;
            dgvUsers.AutoSizeColumnsMode     = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsers.BorderStyle             = BorderStyle.None;
            dgvUsers.BackgroundColor         = Color.White;
            dgvUsers.GridColor               = Color.FromArgb(220, 220, 220);
            dgvUsers.EnableHeadersVisualStyles = false;

            // Header style
            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkSlateGray;
            dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUsers.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvUsers.ColumnHeadersHeight = 30;

            // Row style
            dgvUsers.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvUsers.DefaultCellStyle.SelectionBackColor = Color.SteelBlue;
            dgvUsers.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvUsers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 253);
            dgvUsers.RowTemplate.Height = 26;

            // Columns
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId",       HeaderText = "ID",          Width = 45,  FillWeight = 5  });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUsername",  HeaderText = "Username",    Width = 120, FillWeight = 15 });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEmail",     HeaderText = "Email",       Width = 220, FillWeight = 30 });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colAdmin",     HeaderText = "Admin",       Width = 55,  FillWeight = 7  });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colLastLogin", HeaderText = "Last Login",  Width = 140, FillWeight = 20 });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCreated",   HeaderText = "Created",     Width = 100, FillWeight = 13 });

            dgvUsers.Columns["colAdmin"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvUsers.Columns["colAdmin"].DefaultCellStyle.ForeColor = Color.DarkGoldenrod;
            dgvUsers.Columns["colAdmin"].DefaultCellStyle.Font      = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvUsers.Columns["colId"].DefaultCellStyle.ForeColor    = Color.Gray;
        }

        private void StyleButton(Button btn, string text, Color bg, int x, int y)
        {
            btn.Text      = text;
            btn.BackColor = bg;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font      = new Font("Segoe UI", 9, FontStyle.Bold);
            btn.Cursor    = Cursors.Hand;
            btn.Size      = new Size(120, 30);
            btn.Location  = new Point(x, y);
        }

        // ── Fields ────────────────────────────────────────────────────
        private Panel         panelHeader;
        private Label         lblTitle;
        private Label         lblStatus;
        private SplitContainer splitMain;
        private SplitContainer splitRight;
        private Panel         panelOnline;
        private Label         lblOnlineTitle;
        private Label         lblOnlineCount;
        private ListBox       lstOnline;
        private Button        btnClearOnline;
        private Label         lblPwdTitle;
        private Label         lblPwdCount;
        private ListBox       lstPwdRequests;
        private Button        btnRespondPwd;
        private DataGridView  dgvUsers;
        private Panel         panelActions;
        private Label         lblUserCount;
        private Button        btnRefresh;
        private Button        btnDelete;
        private Button        btnToggleAdmin;
        private Button        btnChangePassword;
        private RichTextBox   rtbLog;
    }
}

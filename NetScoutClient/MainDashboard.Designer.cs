using System.Drawing;
using System.Windows.Forms;

namespace NetScoutClient
{
    partial class MainDashboard
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelHeader    = new Panel();
            lblTitle       = new Label();
            lblAdmin       = new Label();
            lblWelcome     = new Label();

            panelControls  = new Panel();
            lblTargetLabel = new Label();
            txtTarget      = new TextBox();
            btnPing        = new Button();
            btnPort        = new Button();
            btnOS          = new Button();
            btnAggressive  = new Button();
            btnScan        = new Button();
            btnStop        = new Button();
            btnClear       = new Button();

            panelProgress  = new Panel();
            progressBar    = new ProgressBar();
            lblStatus      = new Label();

            dgvResults     = new DataGridView();

            // ── Header ──────────────────────────────────────────────────
            panelHeader.Dock      = DockStyle.Top;
            panelHeader.Height    = 65;
            panelHeader.BackColor = Color.FromArgb(42, 82, 82);

            lblTitle.Text      = "NetScout  —  Dashboard";
            lblTitle.Font      = new Font("Segoe UI", 15, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize  = true;
            lblTitle.Location  = new Point(20, 8);

            lblWelcome.Text      = "Welcome";
            lblWelcome.Font      = new Font("Segoe UI", 9);
            lblWelcome.ForeColor = Color.FromArgb(180, 220, 210);
            lblWelcome.AutoSize  = true;
            lblWelcome.Location  = new Point(22, 38);

            lblAdmin.Text      = "★ ADMIN";
            lblAdmin.Font      = new Font("Segoe UI", 9, FontStyle.Bold);
            lblAdmin.ForeColor = Color.Gold;
            lblAdmin.AutoSize  = true;
            lblAdmin.Anchor    = AnchorStyles.Top | AnchorStyles.Right;
            lblAdmin.Location  = new Point(870, 22);
            lblAdmin.Visible   = false;

            panelHeader.Controls.AddRange(new Control[] { lblTitle, lblWelcome, lblAdmin });

            // ── Controls Panel ───────────────────────────────────────────
            panelControls.Dock      = DockStyle.Top;
            panelControls.Height    = 100;
            panelControls.BackColor = Color.FromArgb(240, 242, 245);
            panelControls.Padding   = new Padding(12, 8, 12, 8);

            lblTargetLabel.Text      = "Target:";
            lblTargetLabel.Font      = new Font("Segoe UI", 9, FontStyle.Bold);
            lblTargetLabel.ForeColor = Color.DarkSlateGray;
            lblTargetLabel.AutoSize  = true;
            lblTargetLabel.Location  = new Point(12, 14);

            txtTarget.Font        = new Font("Segoe UI", 10);
            txtTarget.Location    = new Point(68, 10);
            txtTarget.Size        = new Size(300, 26);
            txtTarget.BorderStyle = BorderStyle.FixedSingle;
            txtTarget.Text        = "192.168.1.1";

            // Row 2 — scan type buttons
            StyleScanBtn(btnPing,       "📡 Ping",        12,  52);
            StyleScanBtn(btnPort,       "🔌 Port Scan",  132,  52);
            StyleScanBtn(btnOS,         "💻 OS Detect",  252,  52);
            StyleScanBtn(btnAggressive, "⚡ Aggressive", 372,  52);

            btnPing.Click       += (s, e) => btnPing_Click(s, e);
            btnPort.Click       += (s, e) => btnPort_Click(s, e);
            btnOS.Click         += (s, e) => btnOS_Click(s, e);
            btnAggressive.Click += (s, e) => btnAggressive_Click(s, e);

            // START button
            btnScan.Text      = "▶  START";
            btnScan.Font      = new Font("Segoe UI", 9, FontStyle.Bold);
            btnScan.BackColor = Color.SeaGreen;
            btnScan.ForeColor = Color.White;
            btnScan.FlatStyle = FlatStyle.Flat;
            btnScan.FlatAppearance.BorderSize = 0;
            btnScan.Size      = new Size(95, 30);
            btnScan.Location  = new Point(680, 52);
            btnScan.Cursor    = Cursors.Hand;
            btnScan.Click    += (s, e) => btnScan_Click(s, e);

            // STOP button
            btnStop.Text      = "■  STOP";
            btnStop.Font      = new Font("Segoe UI", 9, FontStyle.Bold);
            btnStop.BackColor = Color.Crimson;
            btnStop.ForeColor = Color.White;
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.FlatAppearance.BorderSize = 0;
            btnStop.Size      = new Size(85, 30);
            btnStop.Location  = new Point(783, 52);
            btnStop.Cursor    = Cursors.Hand;
            btnStop.Enabled   = false;
            btnStop.Click    += (s, e) => btnStop_Click(s, e);

            // CLEAR button
            btnClear.Text      = "🗑 Clear";
            btnClear.Font      = new Font("Segoe UI", 9);
            btnClear.BackColor = Color.DimGray;
            btnClear.ForeColor = Color.White;
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.Size      = new Size(80, 30);
            btnClear.Location  = new Point(876, 52);
            btnClear.Cursor    = Cursors.Hand;
            btnClear.Click    += (s, e) =>
            {
                dgvResults.Rows.Clear();
                progressBar.Value = 0;
                SetStatus("Ready", Color.DimGray);
            };

            panelControls.Controls.AddRange(new Control[] {
                lblTargetLabel, txtTarget,
                btnPing, btnPort, btnOS, btnAggressive,
                btnScan, btnStop, btnClear
            });

            // ── Progress Panel ───────────────────────────────────────────
            panelProgress.Dock      = DockStyle.Top;
            panelProgress.Height    = 30;
            panelProgress.BackColor = Color.FromArgb(228, 230, 233);
            panelProgress.Padding   = new Padding(8, 5, 8, 5);

            progressBar.Dock  = DockStyle.Left;
            progressBar.Width = 620;
            progressBar.Style = ProgressBarStyle.Continuous;

            lblStatus.Text      = "Ready";
            lblStatus.Font      = new Font("Segoe UI", 9);
            lblStatus.ForeColor = Color.DimGray;
            lblStatus.AutoSize  = true;
            lblStatus.Location  = new Point(636, 7);

            panelProgress.Controls.AddRange(new Control[] { progressBar, lblStatus });

            // ── DataGridView ─────────────────────────────────────────────
            dgvResults.Dock                      = DockStyle.Fill;
            dgvResults.ReadOnly                  = true;
            dgvResults.SelectionMode             = DataGridViewSelectionMode.FullRowSelect;
            dgvResults.MultiSelect               = false;
            dgvResults.AllowUserToAddRows        = false;
            dgvResults.AllowUserToDeleteRows     = false;
            dgvResults.AllowUserToResizeRows     = false;
            dgvResults.RowHeadersVisible         = false;
            dgvResults.AutoSizeColumnsMode       = DataGridViewAutoSizeColumnsMode.Fill;
            dgvResults.BorderStyle               = BorderStyle.None;
            dgvResults.BackgroundColor           = Color.White;
            dgvResults.GridColor                 = Color.FromArgb(220, 220, 220);
            dgvResults.EnableHeadersVisualStyles = false;

            dgvResults.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 82, 82);
            dgvResults.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvResults.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvResults.ColumnHeadersHeight = 30;

            dgvResults.DefaultCellStyle.Font                = new Font("Consolas", 9);
            dgvResults.DefaultCellStyle.SelectionBackColor  = Color.SteelBlue;
            dgvResults.DefaultCellStyle.SelectionForeColor  = Color.White;
            dgvResults.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 253);
            dgvResults.RowTemplate.Height = 24;

            // ── Form ─────────────────────────────────────────────────────
            this.Text          = "NetScout - Dashboard";
            this.Size          = new Size(980, 680);
            this.MinimumSize   = new Size(900, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor     = Color.White;
            this.Load         += MainDashboard_Load;
            this.FormClosing  += MainDashboard_FormClosing;

            this.Controls.Add(dgvResults);
            this.Controls.Add(panelProgress);
            this.Controls.Add(panelControls);
            this.Controls.Add(panelHeader);
        }

        private void StyleScanBtn(Button btn, string text, int x, int y)
        {
            btn.Text      = text;
            btn.Font      = new Font("Segoe UI", 9);
            btn.BackColor = Color.FromArgb(200, 200, 200);
            btn.ForeColor = Color.DarkSlateGray;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Size      = new Size(105, 30);
            btn.Location  = new Point(x, y);
            btn.Cursor    = Cursors.Hand;
        }

        // ── Fields ────────────────────────────────────────────────────
        private Panel       panelHeader;
        private Label       lblTitle;
        private Label       lblAdmin;
        private Label       lblWelcome;
        private Panel       panelControls;
        private Label       lblTargetLabel;
        private TextBox     txtTarget;
        private Button      btnPing;
        private Button      btnPort;
        private Button      btnOS;
        private Button      btnAggressive;
        private Button      btnScan;
        private Button      btnStop;
        private Button      btnClear;
        private Panel       panelProgress;
        private ProgressBar progressBar;
        private Label       lblStatus;
        private DataGridView dgvResults;
    }
}

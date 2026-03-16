using System.Drawing;
using System.Windows.Forms;

namespace NetScoutClient
{
    partial class PortDetailForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelHeader = new Panel();
            lblHost     = new Label();
            panelTop    = new Panel();
            progressBar = new ProgressBar();
            lblStatus   = new Label();
            panelBottom = new Panel();
            btnStop     = new Button();
            btnRescan   = new Button();
            btnClose    = new Button();
            dgvPorts    = new DataGridView();

            // ── Header ──────────────────────────────────────────────────
            panelHeader.Dock      = DockStyle.Top;
            panelHeader.Height    = 50;
            panelHeader.BackColor = Color.FromArgb(42, 82, 82);

            lblHost.Text      = "🔍 Full Port Scan";
            lblHost.Font      = new Font("Segoe UI", 12, FontStyle.Bold);
            lblHost.ForeColor = Color.White;
            lblHost.AutoSize  = true;
            lblHost.Location  = new Point(14, 13);
            panelHeader.Controls.Add(lblHost);

            // ── Progress Panel ───────────────────────────────────────────
            panelTop.Dock      = DockStyle.Top;
            panelTop.Height    = 30;
            panelTop.BackColor = Color.FromArgb(228, 230, 233);
            panelTop.Padding   = new Padding(8, 5, 8, 5);

            progressBar.Dock  = DockStyle.Left;
            progressBar.Width = 460;
            progressBar.Style = ProgressBarStyle.Continuous;

            lblStatus.Text      = "Ready";
            lblStatus.Font      = new Font("Segoe UI", 9);
            lblStatus.ForeColor = Color.DimGray;
            lblStatus.AutoSize  = true;
            lblStatus.Location  = new Point(476, 7);

            panelTop.Controls.AddRange(new Control[] { progressBar, lblStatus });

            // ── DataGridView ─────────────────────────────────────────────
            dgvPorts.Dock                      = DockStyle.Fill;
            dgvPorts.ReadOnly                  = true;
            dgvPorts.SelectionMode             = DataGridViewSelectionMode.FullRowSelect;
            dgvPorts.MultiSelect               = false;
            dgvPorts.AllowUserToAddRows        = false;
            dgvPorts.AllowUserToDeleteRows     = false;
            dgvPorts.AllowUserToResizeRows     = false;
            dgvPorts.RowHeadersVisible         = false;
            dgvPorts.AutoSizeColumnsMode       = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPorts.BorderStyle               = BorderStyle.None;
            dgvPorts.BackgroundColor           = Color.White;
            dgvPorts.GridColor                 = Color.FromArgb(220, 220, 220);
            dgvPorts.EnableHeadersVisualStyles = false;

            dgvPorts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 82, 82);
            dgvPorts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPorts.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvPorts.ColumnHeadersHeight = 30;

            dgvPorts.DefaultCellStyle.Font               = new Font("Consolas", 9);
            dgvPorts.DefaultCellStyle.SelectionBackColor = Color.SteelBlue;
            dgvPorts.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvPorts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 253);
            dgvPorts.RowTemplate.Height = 24;

            dgvPorts.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPort",    HeaderText = "Port",     FillWeight = 15 });
            dgvPorts.Columns.Add(new DataGridViewTextBoxColumn { Name = "colProto",   HeaderText = "Protocol", FillWeight = 15 });
            dgvPorts.Columns.Add(new DataGridViewTextBoxColumn { Name = "colService", HeaderText = "Service",  FillWeight = 40 });
            dgvPorts.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStatus",  HeaderText = "Status",   FillWeight = 20 });

            // ── Bottom Buttons ───────────────────────────────────────────
            panelBottom.Dock      = DockStyle.Bottom;
            panelBottom.Height    = 44;
            panelBottom.BackColor = Color.FromArgb(240, 242, 245);
            panelBottom.Padding   = new Padding(8, 7, 8, 7);

            StyleBtn(btnStop,   "■ Stop",   Color.Crimson,      8,   8);
            StyleBtn(btnRescan, "↺ Rescan", Color.SteelBlue,  100,   8);
            StyleBtn(btnClose,  "✕ Close",  Color.DimGray,    540,   8);

            btnStop.Click   += btnStop_Click;
            btnRescan.Click += btnRescan_Click;
            btnClose.Click  += btnClose_Click;

            panelBottom.Controls.AddRange(new Control[] { btnStop, btnRescan, btnClose });

            // ── Form ─────────────────────────────────────────────────────
            this.Text          = "Port Scan Detail";
            this.Size          = new Size(660, 520);
            this.MinimumSize   = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor     = Color.White;
            this.Load         += PortDetailForm_Load;

            this.Controls.Add(dgvPorts);
            this.Controls.Add(panelBottom);
            this.Controls.Add(panelTop);
            this.Controls.Add(panelHeader);
        }

        private void StyleBtn(Button btn, string text, Color bg, int x, int y)
        {
            btn.Text      = text;
            btn.Font      = new Font("Segoe UI", 9, FontStyle.Bold);
            btn.BackColor = bg;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Size      = new Size(85, 28);
            btn.Location  = new Point(x, y);
            btn.Cursor    = Cursors.Hand;
        }

        private Panel       panelHeader;
        private Label       lblHost;
        private Panel       panelTop;
        private ProgressBar progressBar;
        private Label       lblStatus;
        private DataGridView dgvPorts;
        private Panel       panelBottom;
        private Button      btnStop;
        private Button      btnRescan;
        private Button      btnClose;
    }
}

using System.Drawing;
using System.Windows.Forms;

namespace NetScoutServer
{
    partial class ServerLoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelHeader  = new Panel();
            lblTitle     = new Label();
            lblSubtitle  = new Label();
            lblEmail     = new Label();
            txtEmail     = new TextBox();
            lblPassword  = new Label();
            txtPassword  = new TextBox();
            btnLogin     = new Button();
            lblNote      = new Label();

            // ── Header ──────────────────────────────────────────────────
            panelHeader.Dock      = DockStyle.Top;
            panelHeader.Height    = 90;
            panelHeader.BackColor = Color.FromArgb(42, 82, 82);

            lblTitle.Text      = "NetScout";
            lblTitle.Font      = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize  = true;
            lblTitle.Location  = new Point(130, 12);

            lblSubtitle.Text      = "Admin Server — Sign In";
            lblSubtitle.Font      = new Font("Segoe UI", 10);
            lblSubtitle.ForeColor = Color.FromArgb(180, 220, 210);
            lblSubtitle.AutoSize  = true;
            lblSubtitle.Location  = new Point(130, 52);

            panelHeader.Controls.AddRange(new Control[] { lblTitle, lblSubtitle });

            // ── Email ────────────────────────────────────────────────────
            lblEmail.Text      = "Email Address:";
            lblEmail.Font      = new Font("Segoe UI", 10);
            lblEmail.ForeColor = Color.DarkSlateGray;
            lblEmail.AutoSize  = true;
            lblEmail.Location  = new Point(50, 120);

            txtEmail.Font        = new Font("Segoe UI", 11);
            txtEmail.Location    = new Point(50, 145);
            txtEmail.Size        = new Size(350, 27);
            txtEmail.BorderStyle = BorderStyle.FixedSingle;
            txtEmail.KeyPress   += txtEmail_KeyPress;

            // ── Password ─────────────────────────────────────────────────
            lblPassword.Text      = "Password:";
            lblPassword.Font      = new Font("Segoe UI", 10);
            lblPassword.ForeColor = Color.DarkSlateGray;
            lblPassword.AutoSize  = true;
            lblPassword.Location  = new Point(50, 192);

            txtPassword.Font         = new Font("Segoe UI", 11);
            txtPassword.Location     = new Point(50, 217);
            txtPassword.Size         = new Size(350, 27);
            txtPassword.PasswordChar = '●';
            txtPassword.BorderStyle  = BorderStyle.FixedSingle;
            txtPassword.KeyPress    += txtPassword_KeyPress;

            // ── Login Button ─────────────────────────────────────────────
            btnLogin.Text      = "LOGIN";
            btnLogin.Font      = new Font("Segoe UI", 12, FontStyle.Bold);
            btnLogin.BackColor = Color.FromArgb(42, 82, 82);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Size      = new Size(160, 42);
            btnLogin.Location  = new Point(120, 270);
            btnLogin.Cursor    = Cursors.Hand;
            btnLogin.Click    += btnLogin_Click;

            // ── Note ─────────────────────────────────────────────────────
            lblNote.Text      = "🔒  Admin credentials required";
            lblNote.Font      = new Font("Segoe UI", 9);
            lblNote.ForeColor = Color.Gray;
            lblNote.AutoSize  = true;
            lblNote.Location  = new Point(115, 328);

            // ── Form ─────────────────────────────────────────────────────
            this.Text            = "NetScout — Admin Login";
            this.ClientSize      = new Size(450, 370);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.BackColor       = Color.WhiteSmoke;
            this.Load           += ServerLoginForm_Load;

            this.Controls.AddRange(new Control[]
            {
                panelHeader, lblEmail, txtEmail,
                lblPassword, txtPassword, btnLogin, lblNote
            });
        }

        private Panel   panelHeader;
        private Label   lblTitle;
        private Label   lblSubtitle;
        private Label   lblEmail;
        private TextBox txtEmail;
        private Label   lblPassword;
        private TextBox txtPassword;
        private Button  btnLogin;
        private Label   lblNote;
    }
}

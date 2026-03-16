using System;
using System.Drawing;
using System.Windows.Forms;

namespace NetScoutServer
{
    public partial class ServerLoginForm : Form
    {
        private readonly DatabaseHelper _db = new DatabaseHelper();

        public ServerLoginForm()
        {
            InitializeComponent();
        }

        private void ServerLoginForm_Load(object sender, EventArgs e)
        {
            txtEmail.Focus();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email    = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(email))
            {
                ShowError("Please enter your email.");
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Please enter your password.");
                txtPassword.Focus();
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text    = "Checking...";
            this.Cursor      = Cursors.WaitCursor;
            Application.DoEvents();

            // Validate directly against DB (no TCP needed — server has direct DB access)
            string userData = _db.ValidateUserWithAdmin(email, _db.HashPassword(password));

            this.Cursor      = Cursors.Default;
            btnLogin.Enabled = true;
            btnLogin.Text    = "LOGIN";

            if (userData == null)
            {
                ShowError("Invalid email or password.");
                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            string[] parts  = userData.Split('|');
            bool     isAdmin = parts.Length > 2 && parts[2] == "1";

            if (!isAdmin)
            {
                MessageBox.Show(
                    "Access denied.\n\nThis application is for administrators only.\n\nPlease use the NetScout Client app.",
                    "Admin Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            // Admin confirmed — open dashboard
            var dashboard = new AdminDashboard();
            dashboard.Show();
            this.Hide();

            dashboard.FormClosed += (s, args) => Application.Exit();
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                btnLogin_Click(sender, e);
            }
        }

        private void txtEmail_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                txtPassword.Focus();
            }
        }

        private void ShowError(string msg) =>
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}

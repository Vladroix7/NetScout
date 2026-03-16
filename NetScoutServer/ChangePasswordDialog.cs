using System;
using System.Windows.Forms;

namespace NetScoutServer
{
    public partial class ChangePasswordDialog : Form
    {
        public string NewPassword => txtPassword.Text;

        public ChangePasswordDialog(string username)
        {
            InitializeComponent();
            lblInfo.Text = $"Change password for:  {username}";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            if (txtPassword.Text != txtConfirm.Text)
            {
                MessageBox.Show("Passwords do not match.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirm.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}

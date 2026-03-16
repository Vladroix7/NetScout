using System.Drawing;
using System.Windows.Forms;

namespace NetScoutServer
{
    partial class ChangePasswordDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblInfo    = new Label();
            lblPwd     = new Label();
            txtPassword = new TextBox();
            lblConfirm = new Label();
            txtConfirm = new TextBox();
            btnOK      = new Button();
            btnCancel  = new Button();

            // lblInfo
            lblInfo.AutoSize  = true;
            lblInfo.Font      = new Font("Segoe UI", 10, FontStyle.Bold);
            lblInfo.ForeColor = Color.DarkSlateGray;
            lblInfo.Location  = new Point(20, 20);

            // lblPwd
            lblPwd.Text      = "New Password:";
            lblPwd.AutoSize  = true;
            lblPwd.Font      = new Font("Segoe UI", 9);
            lblPwd.Location  = new Point(20, 60);

            // txtPassword
            txtPassword.Location     = new Point(20, 80);
            txtPassword.Size         = new Size(310, 27);
            txtPassword.Font         = new Font("Segoe UI", 10);
            txtPassword.PasswordChar = '●';

            // lblConfirm
            lblConfirm.Text     = "Confirm Password:";
            lblConfirm.AutoSize = true;
            lblConfirm.Font     = new Font("Segoe UI", 9);
            lblConfirm.Location = new Point(20, 120);

            // txtConfirm
            txtConfirm.Location     = new Point(20, 140);
            txtConfirm.Size         = new Size(310, 27);
            txtConfirm.Font         = new Font("Segoe UI", 10);
            txtConfirm.PasswordChar = '●';

            // btnOK
            btnOK.Text            = "Change Password";
            btnOK.BackColor       = Color.DarkSlateGray;
            btnOK.ForeColor       = Color.White;
            btnOK.FlatStyle       = FlatStyle.Flat;
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Font            = new Font("Segoe UI", 9, FontStyle.Bold);
            btnOK.Location        = new Point(20, 190);
            btnOK.Size            = new Size(150, 32);
            btnOK.Cursor          = Cursors.Hand;
            btnOK.Click          += btnOK_Click;

            // btnCancel
            btnCancel.Text      = "Cancel";
            btnCancel.BackColor = Color.Gray;
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Font      = new Font("Segoe UI", 9);
            btnCancel.Location  = new Point(180, 190);
            btnCancel.Size      = new Size(80, 32);
            btnCancel.Cursor    = Cursors.Hand;
            btnCancel.Click    += btnCancel_Click;

            // Form
            this.Text            = "Change Password";
            this.ClientSize      = new Size(350, 245);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.StartPosition   = FormStartPosition.CenterParent;
            this.BackColor       = Color.WhiteSmoke;
            this.AcceptButton    = btnOK;
            this.CancelButton    = btnCancel;

            this.Controls.Add(lblInfo);
            this.Controls.Add(lblPwd);
            this.Controls.Add(txtPassword);
            this.Controls.Add(lblConfirm);
            this.Controls.Add(txtConfirm);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
        }

        private Label   lblInfo;
        private Label   lblPwd;
        private TextBox txtPassword;
        private Label   lblConfirm;
        private TextBox txtConfirm;
        private Button  btnOK;
        private Button  btnCancel;
    }
}

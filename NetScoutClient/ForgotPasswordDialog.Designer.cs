using System.Drawing;
using System.Windows.Forms;

namespace NetScoutClient
{
    partial class ForgotPasswordDialog
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
            lblEmail   = new Label();
            txtEmail   = new TextBox();
            btnSend    = new Button();
            btnCancel  = new Button();

            // lblInfo
            lblInfo.Text      = "Enter your email and the admin will reset your password.";
            lblInfo.AutoSize  = false;
            lblInfo.Size      = new Size(310, 36);
            lblInfo.Font      = new Font("Segoe UI", 9);
            lblInfo.ForeColor = Color.DimGray;
            lblInfo.Location  = new Point(20, 18);

            // lblEmail
            lblEmail.Text     = "Email Address:";
            lblEmail.AutoSize = true;
            lblEmail.Font     = new Font("Segoe UI", 9);
            lblEmail.Location = new Point(20, 65);

            // txtEmail
            txtEmail.Location = new Point(20, 85);
            txtEmail.Size     = new Size(310, 27);
            txtEmail.Font     = new Font("Segoe UI", 10);

            // btnSend
            btnSend.Text      = "Send Request";
            btnSend.BackColor = Color.DarkSlateGray;
            btnSend.ForeColor = Color.White;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.Font      = new Font("Segoe UI", 9, FontStyle.Bold);
            btnSend.Location  = new Point(20, 135);
            btnSend.Size      = new Size(130, 32);
            btnSend.Cursor    = Cursors.Hand;
            btnSend.Click    += btnSend_Click;

            // btnCancel
            btnCancel.Text      = "Cancel";
            btnCancel.BackColor = Color.Gray;
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Font      = new Font("Segoe UI", 9);
            btnCancel.Location  = new Point(160, 135);
            btnCancel.Size      = new Size(80, 32);
            btnCancel.Cursor    = Cursors.Hand;
            btnCancel.Click    += btnCancel_Click;

            // Form
            this.Text            = "Forgot Password";
            this.ClientSize      = new Size(350, 190);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.StartPosition   = FormStartPosition.CenterParent;
            this.BackColor       = Color.WhiteSmoke;
            this.AcceptButton    = btnSend;
            this.CancelButton    = btnCancel;

            this.Controls.Add(lblInfo);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(btnSend);
            this.Controls.Add(btnCancel);
        }

        private Label   lblInfo;
        private Label   lblEmail;
        private TextBox txtEmail;
        private Button  btnSend;
        private Button  btnCancel;
    }
}

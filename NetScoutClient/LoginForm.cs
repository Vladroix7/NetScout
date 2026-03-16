using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace NetScoutClient
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            txtServerIP.Text = Config.ServerIP;
            txtEmail.Focus();
        }

        private void ApplyServerIP()
        {
            string ip = txtServerIP.Text.Trim();
            if (!string.IsNullOrEmpty(ip))
                Config.ServerIP = ip;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(email) || !email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Please enter a valid email address (e.g. user@example.com)", "Invalid Email",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter your password", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            ApplyServerIP();

            btnLogin.Enabled = false;
            btnLogin.Text = "Connecting...";
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            string result = SendLoginRequest(email, password);

            this.Cursor = Cursors.Default;
            btnLogin.Enabled = true;
            btnLogin.Text = "LOGIN";

            if (result.StartsWith("SUCCESS"))
            {
                // FIX: Parse real userId|username|isAdmin from server response
                string[] parts = result.Split('|');
                // parts: SUCCESS | userId | username | isAdmin
                UserSession.UserId = parts.Length > 1 ? int.Parse(parts[1]) : 0;
                UserSession.Username = parts.Length > 2 ? parts[2] : "User";
                UserSession.IsAdmin = parts.Length > 3 && parts[3] == "1";
                UserSession.Email = email;

                // FIX: Open MainDashboard instead of just showing a message box
                MainDashboard dashboard = new MainDashboard();
                dashboard.Show();
                this.Hide();
            }
            else
            {
                string errorMsg = result.Contains("|") ? result.Split('|')[1] : result;
                MessageBox.Show(errorMsg, "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string SendLoginRequest(string email, string password)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.ReceiveTimeout = Config.ConnectionTimeout;
                    client.SendTimeout = Config.ConnectionTimeout;
                    client.Connect(Config.ServerIP, Config.ServerPort);

                    NetworkStream stream = client.GetStream();
                    string message = $"LOGIN|{email}|{Config.HashPassword(password)}";
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);

                    byte[] buffer = new byte[1024];
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    return Encoding.UTF8.GetString(buffer, 0, bytes);
                }
            }
            catch (SocketException)
            {
                return "ERROR|Cannot connect to server. Make sure the server is running.";
            }
            catch (Exception ex)
            {
                return $"ERROR|{ex.Message}";
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

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                btnLogin_Click(sender, e);
            }
        }

        private void lnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ApplyServerIP();
            RegisterForm registerForm = new RegisterForm();
            registerForm.Show();
            this.Hide();
        }

        private void lnkForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ApplyServerIP();
            using var dlg = new ForgotPasswordDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;

            string email = dlg.Email;
            string result = SendForgotPasswordRequest(email);

            if (result.StartsWith("SUCCESS"))
            {
                MessageBox.Show(
                    "Your password reset request has been sent to the admin.\n" +
                    "Please wait for the admin to reset your password.",
                    "Request Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string errorMsg = result.Contains("|") ? result.Split('|')[1] : result;
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string SendForgotPasswordRequest(string email)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.ReceiveTimeout = Config.ConnectionTimeout;
                    client.SendTimeout    = Config.ConnectionTimeout;
                    client.Connect(Config.ServerIP, Config.ServerPort);

                    NetworkStream stream  = client.GetStream();
                    string message        = $"FORGOT_PASSWORD|{email}";
                    byte[] data           = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);

                    byte[] buffer = new byte[1024];
                    int bytes     = stream.Read(buffer, 0, buffer.Length);
                    return Encoding.UTF8.GetString(buffer, 0, bytes);
                }
            }
            catch (SocketException)
            {
                return "ERROR|Cannot connect to server. Make sure the server is running.";
            }
            catch (Exception ex)
            {
                return $"ERROR|{ex.Message}";
            }
        }
    }
}

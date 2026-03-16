using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NetScoutClient
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {
            txtUsername.Focus();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrEmpty(username))
            { ShowError("Please enter a username"); txtUsername.Focus(); return; }

            if (username.Length < 3)
            { ShowError("Username must be at least 3 characters"); txtUsername.Focus(); return; }

            if (string.IsNullOrEmpty(email))
            { ShowError("Please enter your email address"); txtEmail.Focus(); return; }

            if (!IsValidEmail(email))
            { ShowError("Please enter a valid email address"); txtEmail.Focus(); return; }

            if (string.IsNullOrEmpty(password))
            { ShowError("Please enter a password"); txtPassword.Focus(); return; }

            if (password.Length < 6)
            { ShowError("Password must be at least 6 characters"); txtPassword.Focus(); return; }

            if (password != confirmPassword)
            { ShowError("Passwords do not match"); txtConfirmPassword.Focus(); txtConfirmPassword.SelectAll(); return; }

            btnRegister.Enabled = false;
            btnRegister.Text = "Registering...";
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            string result = SendRegisterRequest(email, password, username);

            this.Cursor = Cursors.Default;
            btnRegister.Enabled = true;
            btnRegister.Text = "REGISTER";

            if (result.StartsWith("SUCCESS"))
            {
                MessageBox.Show(
                    "Registration successful!\n\nYou can now login with your credentials.",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
            }
            else
            {
                string errorMsg = result.Contains("|") ? result.Split('|')[1] : result;
                ShowError(errorMsg);
            }
        }

        private string SendRegisterRequest(string email, string password, string username)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.ReceiveTimeout = Config.ConnectionTimeout;
                    client.SendTimeout = Config.ConnectionTimeout;
                    client.Connect(Config.ServerIP, Config.ServerPort);

                    NetworkStream stream = client.GetStream();
                    string message = $"REGISTER|{email}|{Config.HashPassword(password)}|{username}";
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

        private bool IsValidEmail(string email)
        {
            try { return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"); }
            catch { return false; }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void lnkBackToLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }
    }
}

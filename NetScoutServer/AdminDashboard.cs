using System;
using System.Drawing;
using System.Windows.Forms;

namespace NetScoutServer
{
    public partial class AdminDashboard : Form
    {
        private readonly DatabaseHelper _db = new DatabaseHelper();
        private ServerCore _server;

        public AdminDashboard()
        {
            InitializeComponent();
        }

        // ─────────────────────────────────────────────
        //  STARTUP
        // ─────────────────────────────────────────────

        private void AdminDashboard_Load(object sender, EventArgs e)
        {
            // Set splitter sizes/positions after form has real dimensions
            splitMain.Panel1MinSize    = 180;
            splitMain.Panel2MinSize    = 200;
            splitMain.SplitterDistance = 210;

            splitRight.Panel2MinSize    = 140;
            splitRight.SplitterDistance = 420;

            Log("🚀 NetScout Admin starting...");

            if (!_db.TestConnection())
            {
                MessageBox.Show(
                    "Cannot connect to MySQL!\n\n" +
                    "Check:\n" +
                    "  • MySQL Server is running\n" +
                    "  • Database 'netscout' exists\n" +
                    "  • Password in DatabaseHelper.cs is correct",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("● DB ERROR", Color.Crimson);
            }
            else
            {
                _db.InitializeDatabase();
                EnsureDefaultAdmin();
                LoadUsers();
                Log("✅ Database connected");
            }

            StartServer();
        }

        private void EnsureDefaultAdmin()
        {
            // Only create if not already in DB — RegisterUser checks for duplicates
            bool created = _db.RegisterUser("admin@netscout.com", "admin123", "Admin");
            _db.SetAdminStatus("admin@netscout.com", true);
            if (created) Log("✅ Default admin account created");
        }

        private void StartServer()
        {
            _server = new ServerCore(_db);
            _server.OnLog                += msg => Log(msg);
            _server.OnUserLoggedIn       += OnUserLoggedIn;
            _server.OnPasswordResetRequest += OnPasswordResetRequest;
            _server.Start();
            SetStatus("● RUNNING", Color.LightGreen);
        }

        // ─────────────────────────────────────────────
        //  SERVER EVENTS  (called from background threads)
        // ─────────────────────────────────────────────

        private void OnUserLoggedIn(ConnectedUser user)
        {
            if (InvokeRequired) { Invoke(() => OnUserLoggedIn(user)); return; }

            lstOnline.Items.Insert(0, user.ToString());
            lblOnlineCount.Text = $"Online: {lstOnline.Items.Count}";
            this.Text = $"NetScout — Admin Server v1.0  [{lstOnline.Items.Count} online]";

            // Refresh grid so LastLogin column updates
            LoadUsers();
        }

        private void OnPasswordResetRequest(string email)
        {
            if (InvokeRequired) { Invoke(() => OnPasswordResetRequest(email)); return; }

            // Avoid duplicate entries
            if (!lstPwdRequests.Items.Contains(email))
                lstPwdRequests.Items.Insert(0, email);

            lblPwdCount.Text = $"Pending: {lstPwdRequests.Items.Count}";
            Log($"🔔 Password reset request from: {email}");
        }

        // ─────────────────────────────────────────────
        //  USER GRID
        // ─────────────────────────────────────────────

        private void LoadUsers()
        {
            if (dgvUsers.InvokeRequired) { Invoke(LoadUsers); return; }

            try
            {
                var users = _db.GetAllUsers();
                dgvUsers.Rows.Clear();

                foreach (var u in users)
                {
                    int row = dgvUsers.Rows.Add(
                        u.Id,
                        u.Username,
                        u.Email,
                        u.AdminDisplay,
                        u.LastLoginDisplay,
                        u.CreatedDate.ToString("yyyy-MM-dd")
                    );

                    if (u.IsAdmin)
                        dgvUsers.Rows[row].DefaultCellStyle.ForeColor = Color.DarkGoldenrod;
                }

                lblUserCount.Text = $"Total Users: {users.Count}";
            }
            catch (Exception ex)
            {
                Log($"❌ LoadUsers error: {ex.Message}");
                lblUserCount.Text = "Total Users: ERROR";
            }
        }

        // ─────────────────────────────────────────────
        //  BUTTON HANDLERS
        // ─────────────────────────────────────────────

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadUsers();
            Log("🔄 User list refreshed");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!GetSelectedUser(out int userId, out string username, out string email, out bool isAdmin))
                return;

            if (email == "admin@netscout.com")
            {
                MessageBox.Show("Cannot delete the main admin account.", "Protected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Delete user '{username}' ({email})?\nThis cannot be undone.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            if (_db.DeleteUser(userId))
            {
                Log($"🗑️ Deleted user: {email}");
                LoadUsers();
            }
            else
            {
                MessageBox.Show("Failed to delete user.\n\nIf the user has scan records, those must be deleted first.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnToggleAdmin_Click(object sender, EventArgs e)
        {
            if (!GetSelectedUser(out _, out string username, out string email, out bool isAdmin))
                return;

            if (email == "admin@netscout.com" && isAdmin)
            {
                MessageBox.Show("Cannot remove admin from the main admin account.", "Protected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool newStatus = !isAdmin;
            _db.SetAdminStatus(email, newStatus);
            Log($"👤 {email} → admin: {(newStatus ? "YES" : "NO")}");
            LoadUsers();
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (!GetSelectedUser(out _, out string username, out string email, out _))
                return;

            using var dialog = new ChangePasswordDialog(username);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (_db.ChangePassword(email, dialog.NewPassword))
                {
                    Log($"🔑 Password changed for: {email}");
                    MessageBox.Show($"Password changed successfully for {username}.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to change password.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClearOnline_Click(object sender, EventArgs e)
        {
            lstOnline.Items.Clear();
            lblOnlineCount.Text = "Online: 0";
            this.Text = "NetScout — Admin Server v1.0";
            Log("🧹 Online list cleared");
        }

        private void btnRespondPwd_Click(object sender, EventArgs e)
        {
            if (lstPwdRequests.SelectedItem == null)
            {
                MessageBox.Show("Select a password reset request from the list first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string email = lstPwdRequests.SelectedItem.ToString();

            // Find username from DB
            var users = _db.GetAllUsers();
            var user  = users.Find(u => u.Email == email);
            string displayName = user?.Username ?? email;

            using var dialog = new ChangePasswordDialog(displayName);
            if (dialog.ShowDialog() != DialogResult.OK) return;

            if (_db.ChangePassword(email, dialog.NewPassword))
            {
                lstPwdRequests.Items.Remove(email);
                lblPwdCount.Text = $"Pending: {lstPwdRequests.Items.Count}";
                Log($"🔑 Password reset done for: {email}");
                MessageBox.Show($"Password changed successfully for {displayName}.",
                    "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Failed to change password.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────
        //  HELPERS
        // ─────────────────────────────────────────────

        private bool GetSelectedUser(out int id, out string username, out string email, out bool isAdmin)
        {
            id = 0; username = ""; email = ""; isAdmin = false;

            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            var row = dgvUsers.SelectedRows[0];
            id       = Convert.ToInt32(row.Cells["colId"].Value);
            username = row.Cells["colUsername"].Value.ToString();
            email    = row.Cells["colEmail"].Value.ToString();
            isAdmin  = row.Cells["colAdmin"].Value.ToString() == "✓";
            return true;
        }

        private void Log(string message)
        {
            if (rtbLog.InvokeRequired) { rtbLog.Invoke(() => Log(message)); return; }

            rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
            rtbLog.ScrollToCaret();
        }

        private void SetStatus(string text, Color color)
        {
            if (lblStatus.InvokeRequired) { lblStatus.Invoke(() => SetStatus(text, color)); return; }
            lblStatus.Text      = text;
            lblStatus.ForeColor = color;
        }

        private void AdminDashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            _server?.Stop();
        }
    }
}

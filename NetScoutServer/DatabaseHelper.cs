using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;

namespace NetScoutServer
{
    public class DatabaseHelper
    {
        private string connectionString = "Server=localhost;Port=3306;Database=netscout;Uid=root;Pwd=123456;";

        // ─────────────────────────────────────────────
        //  CONNECTION
        // ─────────────────────────────────────────────

        public bool TestConnection()
        {
            try
            {
                using var conn = new MySqlConnection(connectionString);
                conn.Open();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB connection failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>Ensures all required columns exist (safe to call on every start).</summary>
        public void InitializeDatabase()
        {
            try
            {
                using var conn = new MySqlConnection(connectionString);
                conn.Open();

                var alters = new[]
                {
                    "ALTER TABLE Users ADD COLUMN IsAdmin     TINYINT  DEFAULT 0",
                    "ALTER TABLE Users ADD COLUMN CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP",
                    "ALTER TABLE Users ADD COLUMN LastLogin   DATETIME NULL"
                };

                // Error 1060 = duplicate column (already exists) — safe to ignore
                foreach (var sql in alters)
                    try { using (var cmd = new MySqlCommand(sql, conn)) cmd.ExecuteNonQuery(); }
                    catch (MySqlException ex) when (ex.Number == 1060) { }

                int count;
                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM Users", conn))
                    count = Convert.ToInt32(cmd.ExecuteScalar());

                Console.WriteLine($"DB ready — {count} users");
            }
            catch (Exception ex) { Console.WriteLine($"DB init error: {ex.Message}"); }
        }

        // ─────────────────────────────────────────────
        //  AUTH
        // ─────────────────────────────────────────────

        /// <summary>Returns "userId|username|isAdmin" on success, null on failure.</summary>
        public string ValidateUserWithAdmin(string email, string password)
        {
            try
            {
                using var conn = new MySqlConnection(connectionString);
                conn.Open();

                const string sql = "SELECT Id, Username, Password, IsAdmin FROM Users WHERE Email = @e";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@e", email);
                using var r = cmd.ExecuteReader();

                if (!r.Read()) return null;

                string storedHash = r["Password"].ToString();
                // Client already sends SHA-256 hash — compare directly
                if (storedHash != password) return null;

                int    id      = Convert.ToInt32(r["Id"]);
                string uname   = r["Username"].ToString();
                int    isAdmin = Convert.ToInt32(r["IsAdmin"]);
                r.Close();

                UpdateLastLogin(email);
                return $"{id}|{uname}|{isAdmin}";
            }
            catch (Exception ex) { Console.WriteLine($"ValidateUser error: {ex.Message}"); return null; }
        }

        public bool EmailExists(string email)
        {
            try
            {
                using var conn = new MySqlConnection(connectionString);
                conn.Open();
                using var cmd = new MySqlCommand("SELECT COUNT(*) FROM Users WHERE Email = @e", conn);
                cmd.Parameters.AddWithValue("@e", email);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
            catch { return false; }
        }

        public bool RegisterUser(string email, string password, string username)
        {
            try
            {
                using var conn = new MySqlConnection(connectionString);
                conn.Open();

                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM Users WHERE Email = @e", conn))
                {
                    cmd.Parameters.AddWithValue("@e", email);
                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0) return false;
                }

                const string sql = "INSERT INTO Users (Email, Password, Username, IsAdmin) VALUES (@e, @p, @u, 0)";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@p", password); // client sends pre-hashed
                    cmd.Parameters.AddWithValue("@u", username);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { Console.WriteLine($"RegisterUser error: {ex.Message}"); return false; }
        }

        // ─────────────────────────────────────────────
        //  ADMIN OPERATIONS
        // ─────────────────────────────────────────────

        public List<UserRecord> GetAllUsers()
        {
            var list = new List<UserRecord>();
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            // Ensure optional columns exist before querying
            // Error 1060 = duplicate column (already exists) — safe to ignore
            foreach (var alter in new[]
            {
                "ALTER TABLE Users ADD COLUMN IsAdmin     TINYINT  DEFAULT 0",
                "ALTER TABLE Users ADD COLUMN CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP",
                "ALTER TABLE Users ADD COLUMN LastLogin   DATETIME NULL"
            })
            {
                try { using var c = new MySqlCommand(alter, conn); c.ExecuteNonQuery(); }
                catch (MySqlException ex) when (ex.Number == 1060) { }
            }

            const string sql = "SELECT Id, Email, Username, IsAdmin, LastLogin, CreatedDate FROM Users ORDER BY Id";
            using var cmd = new MySqlCommand(sql, conn);
            using var r   = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new UserRecord
                {
                    Id          = Convert.ToInt32(r["Id"]),
                    Email       = r["Email"].ToString(),
                    Username    = r["Username"].ToString(),
                    IsAdmin     = r["IsAdmin"]     == DBNull.Value ? false      : Convert.ToInt32(r["IsAdmin"]) == 1,
                    LastLogin   = r["LastLogin"]   == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["LastLogin"]),
                    CreatedDate = r["CreatedDate"] == DBNull.Value ? DateTime.Now    : Convert.ToDateTime(r["CreatedDate"])
                });
            }
            return list;
        }

        public bool DeleteUser(int userId)
        {
            try
            {
                using var conn = new MySqlConnection(connectionString);
                conn.Open();
                using var cmd = new MySqlCommand("DELETE FROM Users WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", userId);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex) { Console.WriteLine($"DeleteUser error: {ex.Message}"); return false; }
        }

        public bool SetAdminStatus(string email, bool isAdmin)
        {
            try
            {
                using var conn = new MySqlConnection(connectionString);
                conn.Open();
                using var cmd = new MySqlCommand("UPDATE Users SET IsAdmin = @a WHERE Email = @e", conn);
                cmd.Parameters.AddWithValue("@a", isAdmin ? 1 : 0);
                cmd.Parameters.AddWithValue("@e", email);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex) { Console.WriteLine($"SetAdminStatus error: {ex.Message}"); return false; }
        }

        public bool ChangePassword(string email, string newPassword)
        {
            try
            {
                using var conn = new MySqlConnection(connectionString);
                conn.Open();
                using var cmd = new MySqlCommand("UPDATE Users SET Password = @p WHERE Email = @e", conn);
                cmd.Parameters.AddWithValue("@p", HashPassword(newPassword));
                cmd.Parameters.AddWithValue("@e", email);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex) { Console.WriteLine($"ChangePassword error: {ex.Message}"); return false; }
        }

        // ─────────────────────────────────────────────
        //  HELPERS
        // ─────────────────────────────────────────────

        public void UpdateLastLogin(string email)
        {
            try
            {
                using var conn = new MySqlConnection(connectionString);
                conn.Open();
                using var cmd = new MySqlCommand("UPDATE Users SET LastLogin = @t WHERE Email = @e", conn);
                cmd.Parameters.AddWithValue("@t", DateTime.Now);
                cmd.Parameters.AddWithValue("@e", email);
                cmd.ExecuteNonQuery();
            }
            catch { }
        }

        public string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            var sb = new StringBuilder();
            foreach (byte b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}

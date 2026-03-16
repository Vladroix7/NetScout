using System;

namespace NetScoutServer
{
    public class UserRecord
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedDate { get; set; }

        public string LastLoginDisplay =>
            LastLogin.HasValue ? LastLogin.Value.ToString("yyyy-MM-dd HH:mm") : "Never";

        public string AdminDisplay => IsAdmin ? "✓" : "";
    }
}

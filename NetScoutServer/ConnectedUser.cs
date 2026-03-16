using System;

namespace NetScoutServer
{
    public class ConnectedUser
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime LoginTime { get; set; }

        public override string ToString() =>
            $"● {Username}  ({Email})  {LoginTime:HH:mm:ss}";
    }
}

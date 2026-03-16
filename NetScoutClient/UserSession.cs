namespace NetScoutClient
{
    public static class UserSession
    {
        public static int UserId { get; set; }
        public static string Username { get; set; }
        public static string Email { get; set; }
        public static bool IsAdmin { get; set; }
        public static bool IsLoggedIn => UserId > 0;

        public static void Clear()
        {
            UserId = 0;
            Username = null;
            Email = null;
            IsAdmin = false;
        }
    }
}

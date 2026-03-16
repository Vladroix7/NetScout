using System.Security.Cryptography;
using System.Text;

namespace NetScoutClient
{
    public static class Config
    {
        public static string ServerIP = "127.0.0.1";
        public const int ServerPort = 11111;
        public const int ConnectionTimeout = 5000;

        // Hash password before sending — never send plain text over network
        public static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            var sb = new StringBuilder();
            foreach (byte b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}

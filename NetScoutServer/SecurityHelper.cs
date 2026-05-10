using System;
using System.Security.Cryptography;
using System.Text;

namespace NetScoutServer
{
    /// <summary>
    /// AES-256 encryption for all TCP packets.
    /// Every message sent over the network is encrypted before sending
    /// and decrypted after receiving — Wireshark sees only ciphertext.
    /// </summary>
    public static class SecurityHelper
    {
        // 32-byte shared secret key (AES-256) — must match client exactly
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("NetSc0ut#S3cur3Key!2025$AES256!!");

        /// <summary>
        /// Encrypts a plain text message.
        /// Generates a random 16-byte IV per message, prepends it to the
        /// ciphertext, then Base64-encodes the result for safe TCP transport.
        /// </summary>
        public static string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key  = Key;
            aes.Mode = CipherMode.CBC;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            byte[] plainBytes   = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes  = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            // Layout: [16 bytes IV][ciphertext] → Base64
            byte[] result = new byte[aes.IV.Length + cipherBytes.Length];
            Buffer.BlockCopy(aes.IV,      0, result, 0,             aes.IV.Length);
            Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);
            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Decrypts a Base64-encoded AES-256 ciphertext back to plain text.
        /// Extracts the IV from the first 16 bytes, then decrypts the rest.
        /// </summary>
        public static string Decrypt(string cipherText)
        {
            byte[] fullBytes   = Convert.FromBase64String(cipherText);
            byte[] iv          = new byte[16];
            byte[] cipherBytes = new byte[fullBytes.Length - 16];

            Buffer.BlockCopy(fullBytes, 0,  iv,          0, 16);
            Buffer.BlockCopy(fullBytes, 16, cipherBytes,  0, cipherBytes.Length);

            using var aes       = Aes.Create();
            aes.Key             = Key;
            aes.IV              = iv;
            aes.Mode            = CipherMode.CBC;

            using var decryptor = aes.CreateDecryptor();
            byte[] plainBytes   = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}

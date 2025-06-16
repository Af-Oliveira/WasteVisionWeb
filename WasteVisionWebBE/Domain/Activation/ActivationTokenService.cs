using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace DDDSample1.Domain.Activation
{
    public class ActivationTokenService : IActivationTokenService
    {
        private readonly byte[] _encryptionKey;
        private readonly byte[] _ivKey;
        private readonly int _expirationMinutes;

        public ActivationTokenService(IConfiguration configuration)
        {
            var section = configuration.GetSection("TokenEncryption");

            // Get both keys from configuration
            string encryptionKeyBase64 = section["EncryptionKey"];
            string ivKeyBase64 = section["IvKey"];

            // Convert Base64 keys to bytes
            _encryptionKey = Convert.FromBase64String(encryptionKeyBase64);
            _ivKey = Convert.FromBase64String(ivKeyBase64);

            // Validate key lengths
            if (_encryptionKey.Length != 32) // 256 bits
            {
                throw new ArgumentException("Encryption key must be 32 bytes (256 bits)");
            }
            if (_ivKey.Length != 16) // 128 bits
            {
                throw new ArgumentException("IV key must be 16 bytes (128 bits)");
            }

            _expirationMinutes = section.GetValue<int>("ExpirationMinutes");
        }

        public string GenerateToken(string userId, string email)
        {
            var token = new ActivationToken(userId, email, _expirationMinutes);
            var json = JsonSerializer.Serialize(token);

            return EncryptString(json);
        }

        public ActivationToken ValidateToken(string encryptedToken)
        {
            try
            {
                var json = DecryptString(encryptedToken);
                var token = JsonSerializer.Deserialize<ActivationToken>(json);

                return token;
            }
            catch (Exception ex)
            {
                // Log the exception details if needed
                return null;
            }
        }

        private string EncryptString(string plainText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = _encryptionKey;
                aes.IV = _ivKey;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor())
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    var encrypted = msEncrypt.ToArray();
                    return Convert.ToBase64String(encrypted)
                        .Replace('+', '-')
                        .Replace('/', '_')
                        .Replace("=", ""); // Make it URL safe
                }
            }
        }

        private string DecryptString(string cipherText)
        {
            // Restore padding and URL-safe characters
            cipherText = cipherText
                .Replace('-', '+')
                .Replace('_', '/');

            // Add padding if needed
            switch (cipherText.Length % 4)
            {
                case 2: cipherText += "=="; break;
                case 3: cipherText += "="; break;
            }

            using (var aes = Aes.Create())
            {
                aes.Key = _encryptionKey;
                aes.IV = _ivKey;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor())
                using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}

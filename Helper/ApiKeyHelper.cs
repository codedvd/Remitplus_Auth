using System.Security.Cryptography;
using System.Text;

namespace Remitplus_Accessbank_Service.Helper
{
    public static class ApiKeyHelper
    {
        public static string GenerateApiKey(int lengthBytes = 32, string prefix = null!)
        {
            var keyBytes = new byte[lengthBytes];
            RandomNumberGenerator.Fill(keyBytes);
            string base64 = Base64UrlEncode(keyBytes);

            if (!string.IsNullOrEmpty(prefix))
            {
                return $"{prefix}_{base64}";
            }
            return base64;
        }

        public static string HashApiKey(string apiKey)
        {
            if (apiKey is not null)
            {
                var bytes = Encoding.UTF8.GetBytes(apiKey);
                var hash = SHA256.HashData(bytes);
                return ToHex(hash);
            }
            throw new ArgumentNullException(nameof(apiKey));
        }

        public static bool VerifyApiKey(string providedKey, string storedHash)
        {
            if (providedKey is not null)
            {
                if (storedHash is not null)
                {
                }
                else
                    throw new ArgumentNullException(nameof(storedHash));
                var providedHash = HashApiKey(providedKey);
                return CryptographicEquals(providedHash, storedHash);
            }
            throw new ArgumentNullException(nameof(providedKey));
        }

        private static string Base64UrlEncode(byte[] input)
        {
            var s = Convert.ToBase64String(input);
            s = s.TrimEnd('=');
            s = s.Replace('+', '-').Replace('/', '_');
            return s;
        }

        private static string ToHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        private static bool CryptographicEquals(string a, string b)
        {
            if (a.Length != b.Length) return false;
            var diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}

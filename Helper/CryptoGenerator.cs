using System.Security.Cryptography;

namespace Remitplus_Authentication.Helper
{
    public static class CryptoGenerator
    {
        private const string AllowedChars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+<>?";

        public static string GenerateRandomString(int length)
        {
            var chars = new char[length];
            var bytes = RandomNumberGenerator.GetBytes(length);

            for (int i = 0; i < length; i++)
            {
                chars[i] = AllowedChars[bytes[i] % AllowedChars.Length];
            }

            return new string(chars);
        }

        public static (string Key, string IV) GenerateKeyAndIV()
        {
            string key = GenerateRandomString(32);
            string iv = GenerateRandomString(16);
            return (key, iv);
        }
    }
}

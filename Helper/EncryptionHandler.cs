using System.Security.Cryptography;
using System.Text;

namespace Remitplus_Authentication.Helper;
public interface IEncryptionHandler
{
    string AESDecryptData(string encryptedString);
    string AESEncryptData(string payload);
}

public class EncryptionHandler(IConfiguration config) : IEncryptionHandler
{
    public string AESDecryptData(string encryptedString)
    {
        if (string.IsNullOrWhiteSpace(encryptedString))
            throw new ArgumentException("Encrypted string cannot be null or empty.", nameof(encryptedString));

        var encryptionKey = config["Encryption:Key"]?.Trim();
        var encryptionIV = config["Encryption:Iv"]?.Trim();

        if (string.IsNullOrWhiteSpace(encryptionKey))
            throw new InvalidOperationException("Encryption key is not configured.");

        if (string.IsNullOrWhiteSpace(encryptionIV))
            throw new InvalidOperationException("Encryption IV is not configured.");

        byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);
        byte[] ivBytes = Encoding.UTF8.GetBytes(encryptionIV);

        ValidateKeyAndIVLengths(keyBytes, ivBytes);

        byte[] cipherBytes = Convert.FromBase64String(encryptedString);

        using Aes aesAlg = Aes.Create();
        aesAlg.Key = keyBytes;
        aesAlg.IV = ivBytes;

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msDecrypt = new(cipherBytes);
        using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
        using StreamReader srDecrypt = new(csDecrypt);
        return srDecrypt.ReadToEnd();
    }


    public string AESEncryptData(string payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
            throw new ArgumentException("Payload cannot be null or empty.", nameof(payload));

        var encryptionKey = config["Encryption:Key"]?.Trim();
        var encryptionIV = config["Encryption:Iv"]?.Trim();

        if (string.IsNullOrWhiteSpace(encryptionKey))
            throw new InvalidOperationException("Encryption key is not configured.");

        if (string.IsNullOrWhiteSpace(encryptionIV))
            throw new InvalidOperationException("Encryption IV is not configured.");

        byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);
        byte[] ivBytes = Encoding.UTF8.GetBytes(encryptionIV);

        ValidateKeyAndIVLengths(keyBytes, ivBytes);

        using Aes aesAlg = Aes.Create();
        aesAlg.Key = keyBytes;
        aesAlg.IV = ivBytes;

        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(payload);
        }

        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    private static void ValidateKeyAndIVLengths(byte[] keyBytes, byte[] ivBytes)
    {
        if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32)
            throw new ArgumentException("Invalid key length. Must be 16, 24, or 32 bytes.");

        if (ivBytes.Length != 16)
            throw new ArgumentException("Invalid IV length. Must be exactly 16 bytes.");
    }
}

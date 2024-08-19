using System.Security.Cryptography;

namespace Cryptic.Shared.Common.Cryptography;

public static class AesCbc
{
    private static byte[] Transform(this ICryptoTransform transform, byte[] data)
    {
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
        
        cs.Write(data, 0, data.Length);
        cs.FlushFinalBlock();
        
        return ms.ToArray();
    }
    
    public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        using var encryptor = aes.CreateEncryptor(key, iv);
        
        return encryptor.Transform(data);
    }

    public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        using var decryptor = aes.CreateDecryptor(key, iv);
        
        return decryptor.Transform(data);
    }

    public static byte[] Encrypt(byte[] data, byte[] key)
    {
        var iv = new byte[16];
        RandomNumberGenerator.Fill(iv);

        var ct = Encrypt(data, key, iv);

        var encrypted = new byte[iv.Length + ct.Length];

        Buffer.BlockCopy(iv, 0, encrypted, 0, iv.Length);
        Buffer.BlockCopy(ct, 0, encrypted, iv.Length, ct.Length);
        
        return encrypted;
    }

    public static bool TryDecrypt(byte[] data, byte[] key, out byte[]? output)
    {
        if (data.Length <= 16)
        {
            output = null;
            return false;
        }

        try
        {
            var iv = data[..16];
            var ct = data[iv.Length..];
            
            output = Decrypt(ct, key, iv);
            return true;
        }
        catch
        {
            output = null;
            return false;
        }
    }
}

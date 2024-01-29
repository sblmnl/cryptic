using System.Security.Cryptography;

namespace WebAPI.Common.Security;

public class Pbkdf2Hash
{
    public byte[] Salt { get; }
    public byte[] Hash { get; }
    public int Iterations { get; }
        
    private Pbkdf2Hash(byte[] salt, byte[] hash, int iterations)
    {
        Salt = salt;
        Hash = hash;
        Iterations = iterations;
    }

    public static Pbkdf2Hash New(byte[] password, byte[] salt, int iterations)
    {
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, 32);

        return new(salt, hash, iterations);
    }
    
    public static Pbkdf2Hash New(byte[] password, int iterations)
    {
        var salt = new byte[8];
        RandomNumberGenerator.Fill(salt);

        return New(password, salt, iterations);
    }

    public static bool TryParse(string encodedToken, out Pbkdf2Hash? output)
    {
        output = null;
        var parts = encodedToken.Split('.');

        if (parts.Length != 3)
        {
            return false;
        }
        
        try
        {
            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);
            var iterations = int.Parse(parts[2]);
            
            output = new(salt, hash, iterations);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool Matches(byte[] password)
    {
        var hash = New(password, Salt, Iterations);

        return Hash.SequenceEqual(hash.Hash);
    }
    
    public override string ToString()
    {
        return $"{Convert.ToBase64String(Salt)}." +
               $"{Convert.ToBase64String(Hash)}." +
               Iterations;
    }
}

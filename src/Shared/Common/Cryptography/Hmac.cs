using System.Security.Cryptography;

namespace Cryptic.Shared.Common.Cryptography;

public class Hmac
{
    public byte[] Hash { get; }
    public HmacAlgName Algorithm { get; }

    private Hmac(byte[] hash, HmacAlgName algorithm)
    {
        Hash = hash;
        Algorithm = algorithm;
    }
    
    public bool Verify(byte[] data, byte[] key)
    {
        var hmac = Sign(data, key, Algorithm);

        return CryptographicOperations.FixedTimeEquals(Hash, hmac.Hash);
    }
    
    public override string ToString() => string.Join(".", Algorithm, Convert.ToBase64String(Hash));
    
    public static Hmac Sign(byte[] data, byte[] key, HmacAlgName? algorithm = default)
    {
        var hmacAlgName = algorithm ?? HmacAlgName.HMACSHA256;
        var hmac = hmacAlgName.GetAlgorithm(key);
        
        var signature = hmac.ComputeHash(data);

        return new(signature, hmacAlgName);
    }
    
    public static Hmac Parse(string value)
    {
        var parts = value.Split('.');

        var hmacAlgName = HmacAlgName.Parse(parts[0]);
        var hash = new Span<byte>(new byte[parts[2].Length]);
        
        if (!Convert.TryFromBase64String(parts[1], hash, out var hashLength)
            || hashLength != hmacAlgName.HashLength)
        {
            throw new FormatException("Invalid HMAC!");
        }

        return new Hmac(hash[..hashLength].ToArray(), hmacAlgName);
    }

    public static bool TryParse(string value, out Hmac? output)
    {
        try
        {
            output = Parse(value);
            return true;
        }
        catch
        {
            output = null;
            return false;
        }
    }
    
    public static implicit operator string(Hmac value) => value.ToString();
}
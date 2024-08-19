using System.Security.Cryptography;

namespace Cryptic.Shared.Common.Cryptography;

public record HmacAlgName
{
    public static readonly HmacAlgName HMACMD5 = new("HMACMD5", 16);
    public static readonly HmacAlgName HMACSHA1 = new("HMACSHA1", 20);
    public static readonly HmacAlgName HMACSHA256 = new("HMACSHA256", 32);
    public static readonly HmacAlgName HMACSHA384 = new("HMACSHA384", 48);
    public static readonly HmacAlgName HMACSHA512 = new("HMACSHA512", 64);
    public static readonly HmacAlgName HMACSHA3_256 = new("HMACSHA3-256", 32);
    public static readonly HmacAlgName HMACSHA3_384 = new("HMACSHA3-384", 48);
    public static readonly HmacAlgName HMACSHA3_512 = new("HMACSHA3-512", 64);
    
    public static readonly IReadOnlyDictionary<string, HmacAlgName> Available = new Dictionary<string, HmacAlgName>
    {
        { "HMACMD5", HMACMD5 },
        { "HMACSHA1", HMACSHA1 },
        { "HMACSHA256", HMACSHA256 },
        { "HMACSHA384", HMACSHA384 },
        { "HMACSHA512", HMACSHA512 },
        { "HMACSHA3-256", HMACSHA3_256 },
        { "HMACSHA3-384", HMACSHA3_384 },
        { "HMACSHA3-512", HMACSHA3_512 }
    };

    public string Name { get; } = "HMACSHA256";
    public int HashLength { get; }

    private HmacAlgName(string name, int hashLength)
    {
        Name = name;
        HashLength = hashLength;
    }
    
    public HMAC GetAlgorithm(byte[] key)
    {
        return Name switch
        {
            "HMACMD5" => new HMACMD5(key),
            "HMACSHA1" => new HMACSHA1(key),
            "HMACSHA256" => new HMACSHA256(key),
            "HMACSHA384" => new HMACSHA384(key),
            "HMACSHA512" => new HMACSHA512(key),
            "HMACSHA3-256" => new HMACSHA3_256(key),
            "HMACSHA3-384" => new HMACSHA3_384(key),
            "HMACSHA3-512" => new HMACSHA3_512(key),
            _ => throw new NotSupportedException("Unknown HMAC algorithm!")
        };
    }
    
    public override string ToString()
    {
        return Name;
    }
    
    public static HmacAlgName Parse(string value)
    {
        var hmacAlgName = Available.GetValueOrDefault(value.ToUpper());

        if (hmacAlgName is null)
        {
            throw new NotSupportedException("Unknown HMAC algorithm!");
        }
        
        return hmacAlgName;
    }
    
    public static bool TryParse(string value, out HmacAlgName? output)
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
}

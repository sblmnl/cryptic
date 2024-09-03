using System.Security.Cryptography;
using System.Text.Json.Serialization;

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

    public static readonly List<HmacAlgName> Available =
    [
        HMACMD5,
        HMACSHA1,
        HMACSHA256,
        HMACSHA384,
        HMACSHA512,
        HMACSHA3_256,
        HMACSHA3_384,
        HMACSHA3_512
    ];

    [JsonPropertyName("h")] public string Name { get; init; } = "HMACSHA256";

    [JsonPropertyName("l")] public int HashLength { get; init; }
    
    public HmacAlgName(string name, int hashLength)
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
        var hmacAlgName = Available.FirstOrDefault(x =>
                              string.Equals(x.Name, value.ToUpper(), StringComparison.CurrentCultureIgnoreCase))
                          ?? throw new NotSupportedException("Unknown HMAC algorithm!");

        return hmacAlgName;
    }
}
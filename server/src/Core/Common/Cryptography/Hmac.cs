using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cryptic.Core.Common.Cryptography;

public class Hmac
{
    [JsonPropertyName("hmac")]
    public required byte[] Hash { get; init; }
    
    [JsonPropertyName("h")]
    public required HmacAlgName Algorithm { get; init; }
    
    public static Hmac Sign(byte[] data, byte[] key, HmacAlgName? algorithm = default)
    {
        var h = algorithm ?? HmacAlgName.HMACSHA256;
        var hmac = h.GetAlgorithm(key);
        
        return new()
        {
            Hash = hmac.ComputeHash(data),
            Algorithm = h
        };
    }
    
    public static Hmac Deserialize(string value)
    {
        var jsonString = Encoding.UTF8.GetString(Convert.FromBase64String(value));
        return JsonSerializer.Deserialize<Hmac>(jsonString) ?? throw new FormatException("Invalid HMAC!");
    }
    
    public string Serialize()
    {
        var jsonString = JsonSerializer.Serialize(this);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));
    }
    
    public bool Verify(byte[] data, byte[] key)
    {
        var hmac = Sign(data, key, Algorithm);

        return CryptographicOperations.FixedTimeEquals(Hash, hmac.Hash);
    }
}

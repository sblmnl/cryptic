using System.Security.Cryptography;

namespace Cryptic.Shared.Common.Cryptography;

public record HalgName
{
    public static readonly HalgName MD5 = new("MD5", HashAlgorithmName.MD5);
    public static readonly HalgName SHA1 = new("SHA1", HashAlgorithmName.SHA1);
    public static readonly HalgName SHA256 = new("SHA256", HashAlgorithmName.SHA256);
    public static readonly HalgName SHA384 = new("SHA384", HashAlgorithmName.SHA384);
    public static readonly HalgName SHA512 = new("SHA512", HashAlgorithmName.SHA512);
    public static readonly HalgName SHA3_256 = new("SHA3-256", HashAlgorithmName.SHA3_256);
    public static readonly HalgName SHA3_384 = new("SHA3-384", HashAlgorithmName.SHA3_384);
    public static readonly HalgName SHA3_512 = new("SHA3-512", HashAlgorithmName.SHA3_512);

    public static readonly IReadOnlyDictionary<string, HalgName> Available = new Dictionary<string, HalgName>()
    {
        { "MD5", MD5 },
        { "SHA1", SHA1 },
        { "SHA256", SHA256 },
        { "SHA384", SHA384 },
        { "SHA512", SHA512 },
        { "SHA3-256", SHA3_256 },
        { "SHA3-384", SHA3_384 },
        { "SHA3-512", SHA3_512 }
    };
    
    public string Name { get; } = "SHA256";
    public HashAlgorithmName HashAlgorithmName { get; } = HashAlgorithmName.SHA256;

    private HalgName() { }

    private HalgName(string name, HashAlgorithmName hashAlgorithmName)
    {
        Name = name;
        HashAlgorithmName = hashAlgorithmName;
    }

    public override string ToString()
    {
        return Name;
    }
    
    public static HalgName Parse(string value)
    {
        var halgName = Available.GetValueOrDefault(value);
        
        if (halgName is null)
        {
            throw new NotSupportedException("Unknown hash algorithm!");
        }

        return halgName;
    }
    
    public static bool TryParse(string value, out HalgName? output)
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

    public static implicit operator HashAlgorithmName(HalgName halgName)
    {
        return halgName.HashAlgorithmName;
    }

    public static implicit operator HalgName(HashAlgorithmName hashAlgorithmName)
    {
        return Parse(hashAlgorithmName.Name ?? string.Empty);
    }

    public static implicit operator string(HalgName value)
    {
        return value.ToString();
    }
}

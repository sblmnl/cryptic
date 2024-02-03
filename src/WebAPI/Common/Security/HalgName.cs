using System.Security.Cryptography;

namespace WebAPI.Common.Security;

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
    
    public static bool TryParse(string value, out HalgName? output)
    {
        return Available.TryGetValue(value.ToUpper(), out output);
    }

    public static implicit operator HashAlgorithmName(HalgName halgName)
    {
        return halgName.HashAlgorithmName;
    }

    public static implicit operator HalgName(HashAlgorithmName hashAlgorithmName)
    {
        if (!TryParse(hashAlgorithmName.Name ?? "", out var halgName))
        {
            throw new ArgumentException("Unsupported hash algorithm!");
        }

        return halgName!;
    }

    public static implicit operator string(HalgName value)
    {
        return value.ToString();
    }
}

using System.Security.Cryptography;
using System.Text;

namespace WebAPI.Common.Security;

public record Pbkdf2Hash
{
    public static readonly HalgName DefaultHashAlgorithm = HalgName.SHA256;
    public static readonly Encoding DefaultPasswordEncoding = Encoding.UTF8;
    public const int MaximumIterations = 500000;
    public const int MinimumIterations = 100000;
    public const int DefaultSaltLength = 16;
    public const int DefaultKeyLength = 32;

    public HalgName HashAlgorithm { get; private init; } = DefaultHashAlgorithm;
    public int Iterations { get; private init; } = MaximumIterations;
    public byte[] Salt { get; private init; } = new byte[DefaultSaltLength];
    public byte[] Key { get; private init; } = new byte[DefaultKeyLength];

    private Pbkdf2Hash() { }

    private static Pbkdf2Hash Create(
        byte[] password,
        byte[] salt,
        int iterations,
        HalgName hashAlgorithm,
        int keyLength)
    {
        var key = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            hashAlgorithm,
            keyLength);
        
        return new()
        {
            HashAlgorithm = hashAlgorithm,
            Iterations = iterations,
            Salt = salt,
            Key = key
        };
    }
    
    private static Pbkdf2Hash Create(
        string password,
        byte[] salt,
        int iterations,
        HalgName hashAlgorithm,
        int keyLength)
    {
        return Create(DefaultPasswordEncoding.GetBytes(password), salt, iterations, hashAlgorithm, keyLength);
    }
    
    public static Pbkdf2Hash Create(byte[] password)
    {
        var iterations = Random.Shared.Next(MinimumIterations, MaximumIterations);

        var salt = new Span<byte>(new byte[DefaultSaltLength]);
        RandomNumberGenerator.Fill(salt);

        return Create(password, salt.ToArray(), iterations, DefaultHashAlgorithm, DefaultKeyLength);
    }
    
    public static Pbkdf2Hash Create(string password)
    {
        return Create(DefaultPasswordEncoding.GetBytes(password));
    }

    public static bool TryParse(string value, out Pbkdf2Hash? output)
    {
        output = null;
        var parts = value.Split('.');

        if (parts.Length != 4
            || !HalgName.TryParse(parts[0], out var halgName)
            || halgName is null
            || !int.TryParse(parts[1], out var iterations)
            || iterations > MaximumIterations
            || iterations < MinimumIterations)
        {
            return false;
        }

        var salt = new Span<byte>(new byte[parts[2].Length]);
        var key = new Span<byte>(new byte[parts[3].Length]);

        if (!Convert.TryFromBase64String(parts[2], salt, out var saltLength)
            || !Convert.TryFromBase64String(parts[3], key, out var keyLength))
        {
            return false;
        }

        output = new Pbkdf2Hash()
        {
            HashAlgorithm = halgName,
            Iterations = iterations,
            Salt = salt[..saltLength].ToArray(),
            Key = key[..keyLength].ToArray()
        };
        
        return true;
    }

    public static implicit operator string(Pbkdf2Hash value)
    {
        return value.ToString();
    }
    
    public override string ToString()
    {
        return string.Join(
            ".",
            HashAlgorithm.ToString().ToLower(),
            Iterations,
            Convert.ToBase64String(Salt),
            Convert.ToBase64String(Key));
    }
    
    public bool Verify(byte[] password)
    {
        var hash = Create(password, Salt, Iterations, HashAlgorithm, Key.Length);

        return Key.SequenceEqual(hash.Key);
    }
    
    public bool Verify(string password)
    {
        return Verify(DefaultPasswordEncoding.GetBytes(password));
    }
}

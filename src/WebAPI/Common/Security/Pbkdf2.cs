using System.Security.Cryptography;
using System.Text;

namespace WebAPI.Common.Security;

public static class Pbkdf2
{
    public static readonly HalgName DefaultHashAlgorithm = HalgName.SHA256;
    public const int MaximumIterations = 500000;
    public const int MinimumIterations = 100000;
    public const int DefaultSaltLength = 16;
    public const int DefaultKeyLength = 32;
    
    public record Options
    {
        public HalgName HashAlgorithm { get; private init; } = DefaultHashAlgorithm;
        public int Iterations { get; private init; } = MaximumIterations;
        public byte[] Salt { get; private init; } = new byte[DefaultSaltLength];
        public int KeyLength { get; private init; } = DefaultKeyLength;
    
        private Options() { }

        private Options(HalgName hashAlgorithm, int iterations, byte[] salt, int keyLength)
        {
            HashAlgorithm = hashAlgorithm;
            Iterations = iterations;
            Salt = salt;
            KeyLength = keyLength;
        }

        public static Options Create()
        {
            var salt = new Span<byte>(new byte[DefaultSaltLength]);
            RandomNumberGenerator.Fill(salt);
            
            var iterations = Random.Shared.Next(MinimumIterations, MaximumIterations);

            return new(DefaultHashAlgorithm, iterations, salt.ToArray(), DefaultKeyLength);
        }

        public static bool TryParse(string value, out Options? output)
        {
            output = null;
            var parts = value.Split('.');

            if (parts.Length != 4
                || !HalgName.TryParse(parts[0], out var hashAlgorithm)
                || hashAlgorithm is null
                || !int.TryParse(parts[1], out var iterations)
                || iterations > MaximumIterations
                || iterations < MinimumIterations
                || !int.TryParse(parts[3], out var keyLength))
            {
                return false;
            }

            var salt = new Span<byte>(new byte[parts[2].Length]);

            if (!Convert.TryFromBase64String(parts[2], salt, out var saltLength))
            {
                return false;
            }

            output = new Options
            {
                HashAlgorithm = hashAlgorithm,
                Iterations = iterations,
                Salt = salt[..saltLength].ToArray(),
                KeyLength = keyLength
            };
        
            return true;
        }
        
        public override string ToString()
        {
            return string.Join(
                ".",
                HashAlgorithm.ToString().ToLower(),
                Iterations,
                Convert.ToBase64String(Salt),
                KeyLength);
        }
        
        public static implicit operator string(Options value)
        {
            return value.ToString();
        }
    }

    public record Key
    {
        public Options Options { get; private init; } = Options.Create();
        public byte[] Value { get; private init; } = new byte[DefaultKeyLength];

        private Key() { }

        public static Key Create(byte[] password, Options options)
        {
            var key = Rfc2898DeriveBytes.Pbkdf2(
                password,
                options.Salt,
                options.Iterations,
                options.HashAlgorithm, options.KeyLength);
            
            return new()
            {
                Options = options,
                Value = key
            };
        }

        public static Key Create(byte[] password) => Create(password, Options.Create());
        
        public static Key Create(string password, Encoding encoding) => Create(encoding.GetBytes(password));
        
        public static bool TryParse(string value, out Key? output)
        {
            output = null;
            var parts = value.Split('.');

            if (parts.Length != 5
                || !Options.TryParse(string.Join(".", parts[..4]), out var options)
                || options is null)
            {
                return false;
            }

            var key = new Span<byte>(new byte[parts[4].Length]);

            if (!Convert.TryFromBase64String(parts[4], key, out var keyLength)
                || keyLength != options.KeyLength)
            {
                return false;
            }

            output = new Key()
            {
                Options = options,
                Value = key[..keyLength].ToArray()
            };
        
            return true;
        }
        
        public static implicit operator string(Key value) => value.ToString();
        public static implicit operator byte[](Key value) => value.Value;
        
        public override string ToString() => string.Join(".", Options.ToString(), Convert.ToBase64String(Value));

        public bool Verify(byte[] password)
        {
            return Create(password, Options).Value
                .SequenceEqual(Value);
        }

        public bool Verify(string password, Encoding encoding) => Verify(encoding.GetBytes(password));
    }
}

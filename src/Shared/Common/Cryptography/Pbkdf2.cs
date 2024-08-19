using System.Security.Cryptography;
using System.Text;

namespace Cryptic.Shared.Common.Cryptography;

public static class Pbkdf2
{
    public static readonly HalgName DefaultHashAlgorithm = HalgName.SHA256;
    public const int DefaultIterations = 500000;
    public const int DefaultSaltLength = 16;
    public const int DefaultKeyLength = 32;
    
    public record Options
    {
        public HalgName HashAlgorithm { get; private init; } = DefaultHashAlgorithm;
        public int Iterations { get; private init; } = DefaultIterations;
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

        public static Options Create(
            int keyLength = DefaultKeyLength,
            int saltLength = DefaultSaltLength,
            int iterations = DefaultIterations,
            HalgName? hashAlgorithm = default)
        {
            var salt = new Span<byte>(new byte[saltLength]);
            RandomNumberGenerator.Fill(salt);

            return new(hashAlgorithm ?? DefaultHashAlgorithm, iterations, salt.ToArray(), keyLength);
        }

        public static Options Parse(string value)
        {
            var parts = value.Split('.');

            if (parts.Length != 4)
            {
                throw new FormatException("Invalid PBKDF2 options!");
            }
            
            var hashAlgorithm = HalgName.Parse(parts[0]);
            var salt = Convert.FromBase64String(parts[2]);
            var iterations = int.Parse(parts[1]);
            var keyLength = int.Parse(parts[3]);
            
            return new()
            {
                HashAlgorithm = hashAlgorithm,
                Iterations = iterations,
                Salt = salt,
                KeyLength = keyLength
            };
        }
        
        public static bool TryParse(string value, out Options? output)
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
                options.HashAlgorithm,
                options.KeyLength);
            
            return new()
            {
                Options = options,
                Value = key
            };
        }

        public static Key Create(byte[] password) => Create(password, Options.Create());
        
        public static Key Create(string password, Encoding encoding) => Create(encoding.GetBytes(password));
        
        public static Key Create(string password, Options options, Encoding encoding) =>
            Create(encoding.GetBytes(password), options);

        public static Key Parse(string value)
        {
            var parts = value.Split('.');

            if (parts.Length != 5)
            {
                throw new FormatException("Invalid PBKDF2 key!");
            }
            
            var options = Options.Parse(string.Join(".", parts[..4]));
            var key = Convert.FromBase64String(parts[4]);

            return new()
            {
                Options = options,
                Value = key
            };
        }
        
        public static bool TryParse(string value, out Key? output)
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
        
        public static implicit operator string(Key value) => value.ToString();
        public static implicit operator byte[](Key value) => value.Value;
        
        public override string ToString() => string.Join(".", Options.ToString(), Convert.ToBase64String(Value));

        public bool Verify(byte[] password)
        {
            var key = Create(password, Options);
            
            return CryptographicOperations.FixedTimeEquals(Value, key.Value);
        }

        public bool Verify(string password, Encoding encoding) => Verify(encoding.GetBytes(password));
    }
}

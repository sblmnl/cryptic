using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cryptic.Shared.Common.Cryptography;

public static class Pbkdf2
{
    public static readonly HalgName DefaultHashAlgorithm = HalgName.SHA256;
    public const int DefaultIterations = 500000;
    public const int DefaultSaltLength = 16;
    public const int DefaultKeyLength = 32;
    
    public record Options
    {
        [JsonPropertyName("h")]
        public required HalgName HashAlgorithm { get; init; }
        
        [JsonPropertyName("c")]
        public required int Iterations { get; init; }
        
        [JsonPropertyName("salt")]
        public required byte[] Salt { get; init; }
        
        [JsonPropertyName("dkLen")]
        public required int KeyLength { get; init; }
        
        public static Options Create(
            int keyLength = DefaultKeyLength,
            int saltLength = DefaultSaltLength,
            int iterations = DefaultIterations,
            HalgName? hashAlgorithm = default)
        {
            return new()
            {
                HashAlgorithm = hashAlgorithm ?? DefaultHashAlgorithm,
                Iterations = iterations,
                Salt = RandomNumberGenerator.GetBytes(saltLength),
                KeyLength = keyLength
            };
        }
        
        public static Options Deserialize(string value)
        {
            var jsonString = Encoding.UTF8.GetString(Convert.FromBase64String(value));
            
            return JsonSerializer.Deserialize<Options>(jsonString)
                   ?? throw new FormatException("Invalid PBKDF2 options!");
        }
        
        public string Serialize()
        {
            var jsonString = JsonSerializer.Serialize(this);
            
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));
        }
    }

    public record Key
    {
        [JsonPropertyName("o")]
        public required Options Options { get; init; }
        
        [JsonPropertyName("dk")]
        public required byte[] Value { get; init; }
        
        public static Key Create(byte[] password, Options options)
        {
            return new()
            {
                Options = options,
                Value = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    options.Salt,
                    options.Iterations,
                    options.HashAlgorithm,
                    options.KeyLength)
            };
        }

        public static Key Create(byte[] password) => Create(password, Options.Create());
        
        public static Key Create(string password, Encoding encoding) => Create(encoding.GetBytes(password));
        
        public static Key Create(string password, Options options, Encoding encoding) =>
            Create(encoding.GetBytes(password), options);

        public static implicit operator byte[](Key value) => value.Value;

        public bool Verify(byte[] password)
        {
            var key = Create(password, Options);
            
            return CryptographicOperations.FixedTimeEquals(Value, key.Value);
        }

        public bool Verify(string password, Encoding encoding) => Verify(encoding.GetBytes(password));

        public string Serialize()
        {
            var jsonString = JsonSerializer.Serialize(this);
            
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));
        }

        public static Key Deserialize(string value)
        {
            var jsonString = Encoding.UTF8.GetString(Convert.FromBase64String(value));
            
            return JsonSerializer.Deserialize<Key>(jsonString)
                   ?? throw new FormatException("Invalid PBKDF2 key!");
        }
    }
}

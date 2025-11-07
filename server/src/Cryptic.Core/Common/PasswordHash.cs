using Cryptic.Core.Cryptography;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Cryptic.Core.Common;

public record PasswordHash : DerivedKey<object>
{
    private bool VerifyArgon2(byte[] passwordBytes)
    {
        var derivedKey = Argon2DerivedKey.Create(passwordBytes, (Argon2Parameters)Parameters, Key.Length);
        return CryptographicOperations.FixedTimeEquals(Key, derivedKey.Key);
    }

    public bool Verify(string password)
    {
        byte[] passwordBytes = Encoding.Unicode.GetBytes(password);

        return Function switch
        {
            KeyDerivationFunctionName.Argon2 => VerifyArgon2(passwordBytes),
            _ => false,
        };
    }

    public string Serialize() => DerivedKeySerializer.Serialize(this);

    public static PasswordHash Create(string password)
    {
        byte[] passwordBytes = Encoding.Unicode.GetBytes(password);

        var parameters = Argon2Parameters.OwaspMostCpuIntensive();
        var derivedKey = Argon2DerivedKey.Create(passwordBytes, parameters);

        return new()
        {
            Key = derivedKey.Key,
            Function = KeyDerivationFunctionName.Argon2,
            Parameters = derivedKey.Parameters,
        };
    }

    private static PasswordHash DeserializeArgon2(DerivedKey<object> derivedKey)
    {
        return new()
        {
            Key = derivedKey.Key,
            Function = derivedKey.Function,
            Parameters = ((JObject)derivedKey.Parameters).ToObject<Argon2Parameters>()!,
        };
    }

    public static PasswordHash Deserialize(string serializedString)
    {
        var derivedKey = DerivedKeySerializer.Deserialize<object>(serializedString);

        return derivedKey.Function switch
        {
            KeyDerivationFunctionName.Argon2 => DeserializeArgon2(derivedKey),
            _ => new PasswordHash
            {
                Key = derivedKey.Key,
                Function = derivedKey.Function,
                Parameters = derivedKey.Parameters,
            },
        };
    }
}

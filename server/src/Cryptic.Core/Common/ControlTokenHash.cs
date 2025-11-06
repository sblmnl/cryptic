using Cryptic.Core.Cryptography;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

namespace Cryptic.Core.Common;

public record ControlTokenHash : DerivedKey<object>
{
    private bool VerifyArgon2(byte[] controlTokenBytes)
    {
        var derivedKey = Argon2DerivedKey.Create(controlTokenBytes, (Argon2Parameters)Parameters, Key.Length);
        return CryptographicOperations.FixedTimeEquals(Key, derivedKey.Key);
    }

    public bool Verify(ControlToken controlToken)
    {
        return Function switch
        {
            KeyDerivationFunctionName.Argon2 => VerifyArgon2(controlToken.Value),
            _ => false,
        };
    }

    public string Serialize() => DerivedKeySerializer.Serialize(this);

    public static ControlTokenHash Create(ControlToken controlToken)
    {
        var parameters = Argon2Parameters.OwaspMostCpuIntensive();
        var derivedKey = Argon2DerivedKey.Create(controlToken.Value, parameters);

        return new()
        {
            Function = KeyDerivationFunctionName.Argon2,
            Parameters = derivedKey.Parameters,
            Key = derivedKey.Key,
        };
    }

    private static ControlTokenHash DeserializeArgon2(DerivedKey<object> derivedKey)
    {
        return new()
        {
            Key = derivedKey.Key,
            Function = derivedKey.Function,
            Parameters = ((JObject)derivedKey.Parameters).ToObject<Argon2Parameters>()!,
        };
    }

    public static ControlTokenHash Deserialize(string serializedString)
    {
        var derivedKey = DerivedKeySerializer.Deserialize<object>(serializedString);

        return derivedKey.Function switch
        {
            KeyDerivationFunctionName.Argon2 => DeserializeArgon2(derivedKey),
            _ => new ControlTokenHash
            {
                Key = derivedKey.Key,
                Function = derivedKey.Function,
                Parameters = derivedKey.Parameters,
            },
        };
    }
}

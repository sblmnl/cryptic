namespace Cryptic.Core.Cryptography;

public record Argon2DerivedKey : DerivedKey<Argon2Parameters>
{
    public static DerivedKey<Argon2Parameters> Create(
        byte[] inputData,
        Argon2Parameters parameters,
        int keyLength = 32)
    {
        var generator = new Org.BouncyCastle.Crypto.Generators.Argon2BytesGenerator();
        generator.Init(parameters);

        byte[] key = new byte[keyLength];
        generator.GenerateBytes(inputData, key);

        return new Argon2DerivedKey
        {
            Key = key,
            Function = KeyDerivationFunctionName.Argon2,
            Parameters = parameters,
        };
    }
}

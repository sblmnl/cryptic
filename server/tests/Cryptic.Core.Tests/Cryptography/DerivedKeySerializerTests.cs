namespace Cryptic.Core.Tests.Cryptography;

public class DerivedKeySerializerTests
{
    [Fact]
    public void Serialize_Deserialize_RoundTrip_PreservesFunction()
    {
        var key = new DerivedKey<string>
        {
            Function = KeyDerivationFunctionName.Argon2,
            Parameters = "test-parameters",
            Key = [1, 2, 3, 4],
        };

        var serialized = DerivedKeySerializer.Serialize(key);
        var deserialized = DerivedKeySerializer.Deserialize<string>(serialized);

        Assert.Equal(key.Function, deserialized.Function);
    }

    [Fact]
    public void Serialize_Deserialize_RoundTrip_PreservesKey()
    {
        byte[] originalKey = [10, 20, 30, 40, 50];
        var key = new DerivedKey<string>
        {
            Function = KeyDerivationFunctionName.Argon2,
            Parameters = "params",
            Key = originalKey,
        };

        var serialized = DerivedKeySerializer.Serialize(key);
        var deserialized = DerivedKeySerializer.Deserialize<string>(serialized);

        Assert.Equal(originalKey, deserialized.Key);
    }

    [Fact]
    public void Serialize_ProducesValidBase64String()
    {
        var key = new DerivedKey<string>
        {
            Function = KeyDerivationFunctionName.Argon2,
            Parameters = "params",
            Key = [1, 2, 3],
        };

        var serialized = DerivedKeySerializer.Serialize(key);

        var bytes = Convert.FromBase64String(serialized); // must not throw
        Assert.NotEmpty(bytes);
    }

    [Fact]
    public void Deserialize_InvalidBase64_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() =>
            DerivedKeySerializer.Deserialize<string>("not-valid-base64!!!"));
    }

    [Fact]
    public void Deserialize_ValidBase64ButInvalidJson_Throws()
    {
        var invalidJson = Convert.ToBase64String("not-json"u8.ToArray());
        Assert.ThrowsAny<Exception>(() =>
            DerivedKeySerializer.Deserialize<string>(invalidJson));
    }
}
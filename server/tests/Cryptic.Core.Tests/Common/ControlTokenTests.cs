namespace Cryptic.Core.Tests.Common;

public class ControlTokenTests
{
    [Fact]
    public void Create_ProducesTokenWithDefaultLength()
    {
        var token = ControlToken.Create();
        Assert.Equal(16, token.Value.Length);
    }

    [Theory]
    [InlineData(16)]
    [InlineData(32)]
    [InlineData(64)]
    public void Create_ProducesTokenWithSpecifiedLength(int length)
    {
        var token = ControlToken.Create(length);
        Assert.Equal(length, token.Value.Length);
    }

    [Fact]
    public void Create_ProducesUniqueTokensEachCall()
    {
        var token1 = ControlToken.Create();
        var token2 = ControlToken.Create();
        Assert.False(token1.Value.SequenceEqual(token2.Value));
    }

    [Fact]
    public void ToString_ReturnsBase64OfValue()
    {
        var token = ControlToken.Create();
        var expected = Convert.ToBase64String(token.Value);
        Assert.Equal(expected, token.ToString());
    }

    [Fact]
    public void Parse_RoundTripsViaToString()
    {
        var original = ControlToken.Create();
        var tokenString = original.ToString();
        var parsed = ControlToken.Parse(tokenString);
        Assert.True(original.Value.SequenceEqual(parsed.Value));
    }

    [Fact]
    public void TryParse_ValidBase64_ReturnsTrue()
    {
        var tokenString = ControlToken.Create().ToString();
        var success = ControlToken.TryParse(tokenString, out var token);
        Assert.True(success);
        Assert.NotNull(token);
    }

    [Fact]
    public void TryParse_InvalidBase64_ReturnsFalse()
    {
        var success = ControlToken.TryParse("not-valid-base64!!!", out var token);
        Assert.False(success);
        Assert.Null(token);
    }
}
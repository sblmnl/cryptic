using System.Security.Cryptography;

namespace Cryptic.Core.Common;

public record ControlToken
{
    public byte[] Value { get; }

    private ControlToken(byte[] value)
    {
        Value = value;
    }

    public override string ToString() => Convert.ToBase64String(Value);

    public static ControlToken Create(int length = 16)
    {
        byte[] value = new byte[length];
        RandomNumberGenerator.Fill(value);
        return new(value);
    }

    public static ControlToken Parse(string tokenString) => new(Convert.FromBase64String(tokenString));

    public static bool TryParse(string tokenString, out ControlToken? token)
    {
        try
        {
            token = Parse(tokenString);
            return true;
        }
        catch
        {
            token = null;
            return false;
        }
    }
}

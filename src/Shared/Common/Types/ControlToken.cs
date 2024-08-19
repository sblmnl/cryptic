using System.Security.Cryptography;

namespace Cryptic.Shared.Common.Types;

public record ControlToken
{
    public const int DefaultLength = 16;

    public byte[] Value { get; }

    private ControlToken(byte[] value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Convert.ToBase64String(Value);
    }
        
    public static ControlToken Create(int length = DefaultLength)
    {
        var token = new byte[length];
        RandomNumberGenerator.Fill(token);
        return new(token);
    }
    
    public static ControlToken Parse(string value)
    {
        return new (Convert.FromBase64String(value));
    }

    public static bool TryParse(string value, out ControlToken? output)
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

    public static implicit operator string(ControlToken value)
    {
        return value.ToString();
    }
}

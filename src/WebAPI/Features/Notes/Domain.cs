using System.Security.Cryptography;
using WebAPI.Common.Security;

namespace WebAPI.Features.Notes;

public static class Domain
{
    public record DeleteAfter
    {
        public record Reading(bool DoNotWarn) : DeleteAfter;
        public record Time(DateTimeOffset DeleteAt) : DeleteAfter;

        private DeleteAfter() { }

        public static DeleteAfter From(DateTimeOffset? deleteAt, bool doNotWarn)
        {
            return deleteAt is null
                ? new Reading(doNotWarn)
                : new Time(deleteAt ?? default);
        }
    }

    public record ControlToken
    {
        public const int MaximumLength = 32;
        public const int DefaultLength = 16;
        
        public byte[] Value { get; }

        private ControlToken(byte[] value)
        {
            Value = value;
        }

        public static ControlToken New(int length = DefaultLength)
        {
            var token = new byte[length];
            RandomNumberGenerator.Fill(token);
            return new(token);
        }

        public static bool TryParse(string value, out ControlToken? output)
        {
            var decoded = new Span<byte>(new byte[MaximumLength]);

            if (!Convert.TryFromBase64String(value, decoded, out var bytesWritten))
            {
                output = null;
                return false;
            }

            output = new(decoded[..bytesWritten].ToArray());
            return true;
        }

        public override string ToString()
        {
            return Convert.ToBase64String(Value);
        }
    }

    public record Note
    {
        public required Guid Id { get; init; }
        public required string Content { get; init; }
        public required DeleteAfter DeleteAfter { get; init; }
        public required Pbkdf2Hash ControlTokenHash { get; init; }
    }
}

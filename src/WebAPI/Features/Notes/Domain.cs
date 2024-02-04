using System.Security.Cryptography;
using System.Text;
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
        
        public static ControlToken New(int length = DefaultLength)
        {
            var token = new byte[length];
            RandomNumberGenerator.Fill(token);
            return new(token);
        }

        public static bool TryParse(string value, out ControlToken? output)
        {
            var decoded = new Span<byte>(new byte[value.Length]);

            if (!Convert.TryFromBase64String(value, decoded, out var length))
            {
                output = null;
                return false;
            }

            output = new(decoded[..length].ToArray());
            return true;
        }

        public static implicit operator string(ControlToken value)
        {
            return value.ToString();
        }
    }
    
    public abstract record Note
    {
        public record Unprotected : Note
        {
            public Unprotected(Guid id, string content, DeleteAfter deleteAfter, Pbkdf2.Key controlTokenHash)
                : base(id, content, deleteAfter, controlTokenHash)
            {
            }
        }

        public record Protected : Note
        {
            public Pbkdf2.Key PasswordHash { get; private init; }
            public Pbkdf2.Options KeyOptions { get; private init; }
            
            public Protected(
                Guid id,
                string content,
                DeleteAfter deleteAfter,
                Pbkdf2.Key controlTokenHash,
                Pbkdf2.Key passwordHash,
                Pbkdf2.Options keyOptions) : base(id, content, deleteAfter, controlTokenHash)
            {
                PasswordHash = passwordHash;
                KeyOptions = keyOptions;
            }

            public static Protected New(Unprotected note, string password)
            {
                var hash = Pbkdf2.Key.Create(password, Encoding.UTF8);
                var key = Pbkdf2.Key.Create(password, Encoding.UTF8);

                while (key.Value.SequenceEqual(hash.Value))
                {
                    key = Pbkdf2.Key.Create(password, Encoding.UTF8);
                }

                var ct = AesCbc.Encrypt(Encoding.UTF8.GetBytes(note.Content), key.Value);
                
                return new(
                    note.Id,
                    Convert.ToBase64String(ct),
                    note.DeleteAfter,
                    note.ControlTokenHash,
                    hash,
                    key.Options);
            }

            public bool TryDecrypt(string password, out Unprotected? output)
            {
                if (!PasswordHash.Verify(password, Encoding.UTF8))
                {
                    output = null;
                    return false;
                }
                
                var key = Pbkdf2.Key.Create(Encoding.UTF8.GetBytes(password), KeyOptions);
                
                if (!AesCbc.TryDecrypt(Convert.FromBase64String(Content), key.Value, out var pt)
                    || pt is null)
                {
                    output = null;
                    return false;
                }

                output = new Unprotected(Id, Encoding.UTF8.GetString(pt), DeleteAfter, ControlTokenHash);
                return true;
            }
        }
        
        public Guid Id { get; protected init; }
        public string Content { get; protected init; }
        public DeleteAfter DeleteAfter { get; protected init; }
        public Pbkdf2.Key ControlTokenHash { get; protected init; }

        protected Note(Guid id, string content, DeleteAfter deleteAfter, Pbkdf2.Key controlTokenHash)
        {
            Id = id;
            Content = content;
            DeleteAfter = deleteAfter;
            ControlTokenHash = controlTokenHash;
        }
    }
}

using System.Text;
using System.Text.Json;

namespace Cryptic.Shared.Features.Notes;

public static partial class DomainErrors
{
    public static readonly CodedError IncorrectPassword = new()
    {
        Code = "Cryptic.Notes.IncorrectPassword",
        Message = "The provided password was incorrect!"
    };
}

public static partial class Domain
{

    public abstract record NoteContent
    {
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;
        
        public required string Value { get; init; }

        public record Plaintext : NoteContent;

        public record Encrypted : NoteContent
        {
            public required Hmac Signature { get; init; }
            public required Pbkdf2.Options EncryptionKeyOptions { get; init; }
            public required Pbkdf2.Options SigningKeyOptions { get; init; }
            
            public Result<Plaintext> Decrypt(string password, Encoding? encoding = null)
            {
                var encoder = encoding ?? DefaultEncoding;
                
                var ciphertextBytes = Convert.FromBase64String(Value);
                var signingKey = Pbkdf2.Key.Create(password, SigningKeyOptions, encoder);
                
                if (!Signature.Verify(ciphertextBytes, signingKey))
                {
                    return new Result<Plaintext>.Fail(DomainErrors.IncorrectPassword);
                }
                
                var encryptionKey = Pbkdf2.Key.Create(password, EncryptionKeyOptions, encoder);
                var plaintextBytes = AesCbc.Decrypt(ciphertextBytes, encryptionKey);

                return new Result<Plaintext>.Ok(new()
                {
                    Value = encoder.GetString(plaintextBytes)
                });
            }
            
            public static Encrypted Create(string content, string password, Encoding? encoding = null)
            {
                var encoder = encoding ?? DefaultEncoding;
                
                var encryptionKey = Pbkdf2.Key.Create(password, encoder);
                var signingKeyOptions = Pbkdf2.Options.Create(64);
                var signingKey = Pbkdf2.Key.Create(password, signingKeyOptions, encoder);
                
                var plaintextBytes = encoder.GetBytes(content);
                var ciphertextBytes = AesCbc.Encrypt(plaintextBytes, encryptionKey);
                var signature = Hmac.Sign(ciphertextBytes, signingKey);

                Console.WriteLine(JsonSerializer.Serialize(signature));
                
                return new()
                {
                    Value = Convert.ToBase64String(ciphertextBytes),
                    Signature = signature,
                    EncryptionKeyOptions = encryptionKey.Options,
                    SigningKeyOptions = signingKey.Options
                };
            }
        }
        
        public static implicit operator string(NoteContent content) => content.Value;
    }

    public record Note
    {
        public required Guid Id { get; init; }
        public required NoteContent Content { get; init; }
        public required DeleteAfter DeleteAfter { get; init; }
        public required Pbkdf2.Key ControlTokenHash { get; init; }
    }
}

public static class NoteExtensions
{
    public static bool HasDeleteAfterTimePassed(this Domain.Note note) =>
        note.DeleteAfter.Time <= DateTimeOffset.UtcNow;
}

public static partial class DataModels
{
    public record Note
    {
        public required Guid Id { get; init; }
        public required string Content { get; init; }
        public required bool DeleteOnReceipt { get; init; }
        public required DateTimeOffset DeleteAfterTime { get; init; }
        public required string ControlTokenHash { get; init; }
        public required string? Signature { get; init; }
        public required string? EncryptionKeyOptions { get; init; }
        public required string? SigningKeyOptions { get; init; }
    }
    
    public static Domain.Note ToDomainType(this Note rawNote)
    {
        var isEncrypted = rawNote.Signature is not null
                          && rawNote.EncryptionKeyOptions is not null
                          && rawNote.SigningKeyOptions is not null;

        return new Domain.Note
        {
            Id = rawNote.Id,
            Content = isEncrypted switch
            {
                true => new Domain.NoteContent.Encrypted
                {
                    Value = rawNote.Content,
                    Signature = Hmac.Deserialize(rawNote.Signature!),
                    SigningKeyOptions = Pbkdf2.Options.Deserialize(rawNote.SigningKeyOptions!),
                    EncryptionKeyOptions = Pbkdf2.Options.Deserialize(rawNote.EncryptionKeyOptions!),
                },
                _ => new Domain.NoteContent.Plaintext
                {
                    Value = rawNote.Content
                }
            },
            DeleteAfter = new DeleteAfter
            {
                Receipt = rawNote.DeleteOnReceipt,
                Time = rawNote.DeleteAfterTime
            },
            ControlTokenHash = Pbkdf2.Key.Deserialize(rawNote.ControlTokenHash),
        };
    }

    public static Note ToStorageType(this Domain.Note note)
    {
        var isEncrypted = note.Content is Domain.NoteContent.Encrypted;
        
        var encryptedContent = isEncrypted
            ? note.Content as Domain.NoteContent.Encrypted
            : null;

        return new()
        {
            Id = note.Id,
            Content = note.Content,
            DeleteOnReceipt = note.DeleteAfter.Receipt,
            DeleteAfterTime = note.DeleteAfter.Time,
            ControlTokenHash = note.ControlTokenHash.Serialize(),
            Signature = encryptedContent?.Signature.Serialize(),
            EncryptionKeyOptions = encryptedContent?.EncryptionKeyOptions.Serialize(),
            SigningKeyOptions = encryptedContent?.SigningKeyOptions.Serialize()
        };
    }
}

using System.Security.Cryptography;
using System.Text;

namespace Cryptic.Shared.Features.Notes;

public static partial class DomainErrors
{
    public static readonly Error InvalidCiphertext = new(
        "Cryptic.Notes.DomainErrors.InvalidCiphertext",
        "Invalid ciphertext!");
    
    public static readonly Error SignatureMismatch = new(
        "Cryptic.Notes.DomainErrors.SignatureMismatch",
        "The signatures do not match!");
    
    public static readonly Error DecryptionFailed = new(
        "Cryptic.Notes.DomainErrors.DecryptionFailed",
        "An error occurred while decrypting the note!");
}

public static partial class Domain
{
    public abstract record Note
    {
        public static readonly Encoding DefaultEncoding = Encoding.Unicode;
        
        public record Unprotected : Note
        {
            public Protected Encrypt(string password, Encoding? encoding = default)
            {
                var encryptionKey = Pbkdf2.Key.Create(password, DefaultEncoding);
                
                var signingKeyOptions = Pbkdf2.Options.Create(64);
                var signingKey = Pbkdf2.Key.Create(password, signingKeyOptions, DefaultEncoding);
                var iv = RandomNumberGenerator.GetBytes(16);

                var plaintextBytes = DefaultEncoding.GetBytes(Content);
                var ciphertextBytes = AesCbc.Encrypt(plaintextBytes, encryptionKey, iv);
                var signature = Hmac.Sign(signingKey, ciphertextBytes);
                
                return new Protected
                {
                    Id = Id,
                    Content = Convert.ToBase64String(ciphertextBytes),
                    DeleteAfter = DeleteAfter,
                    ControlTokenHash = ControlTokenHash,
                    Signature = signature,
                    EncryptionKeyOptions = encryptionKey.Options,
                    SigningKeyOptions = signingKeyOptions
                };
            }
        }

        public record Protected : Note
        {
            public required Hmac Signature { get; init; }
            public required Pbkdf2.Options EncryptionKeyOptions { get; init; }
            public required Pbkdf2.Options SigningKeyOptions { get; init; }

            public Result<Unprotected> Decrypt(string password)
            {
                var signingKey = Pbkdf2.Key.Create(password, SigningKeyOptions, Encoding.Unicode);
                
                var contentBytes = new Span<byte>(new byte[Content.Length]);

                if (!Convert.TryFromBase64String(Content, contentBytes, out var contentLength))
                {
                    return new Result<Unprotected>.Failure(DomainErrors.InvalidCiphertext);
                }

                var ciphertextBytes = contentBytes[..contentLength].ToArray();

                if (Signature.Verify(ciphertextBytes, signingKey))
                {
                    return new Result<Unprotected>.Failure(DomainErrors.SignatureMismatch);
                }
                
                var encryptionKey = Pbkdf2.Key.Create(password, EncryptionKeyOptions, DefaultEncoding);

                if (!AesCbc.TryDecrypt(ciphertextBytes, encryptionKey, out var plaintextBytes)
                    || plaintextBytes is null)
                {
                    return new Result<Unprotected>.Failure(DomainErrors.DecryptionFailed);
                }
                
                var plaintextContent = DefaultEncoding.GetString(plaintextBytes);

                return new Result<Unprotected>.Success(new()
                {
                    Id = Id,
                    Content = plaintextContent,
                    DeleteAfter = DeleteAfter,
                    ControlTokenHash = ControlTokenHash
                });
            }
        }
    
        public required Guid Id { get; init; }
        public required string Content { get; init; }
        public required DeleteAfter DeleteAfter { get; init; }
        public required Pbkdf2.Key ControlTokenHash { get; init; }
    }
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
        var isProtected = rawNote.Signature is not null
                          && rawNote.EncryptionKeyOptions is not null
                          && rawNote.SigningKeyOptions is not null;

        if (isProtected)
        {
            return new Domain.Note.Protected
            {
                Id = rawNote.Id,
                Content = rawNote.Content,
                DeleteAfter = new DeleteAfter
                {
                    Receipt = rawNote.DeleteOnReceipt,
                    Time = rawNote.DeleteAfterTime
                },
                ControlTokenHash = Pbkdf2.Key.Parse(rawNote.ControlTokenHash),
                Signature = Hmac.Parse(rawNote.Signature!),
                SigningKeyOptions = Pbkdf2.Options.Parse(rawNote.SigningKeyOptions!),
                EncryptionKeyOptions = Pbkdf2.Options.Parse(rawNote.EncryptionKeyOptions!),
            };
        }
        
        return new Domain.Note.Unprotected
        {
            Id = rawNote.Id,
            Content = rawNote.Content,
            DeleteAfter = new DeleteAfter
            {
                Receipt = rawNote.DeleteOnReceipt,
                Time = rawNote.DeleteAfterTime
            },
            ControlTokenHash = Pbkdf2.Key.Parse(rawNote.ControlTokenHash),
        };
    }

    public static Note ToStorageType(this Domain.Note note)
    {
        return note switch
        {
            Domain.Note.Protected protectedNote => new()
            {
                Id = protectedNote.Id,
                Content = protectedNote.Content,
                DeleteOnReceipt = protectedNote.DeleteAfter.Receipt,
                DeleteAfterTime = protectedNote.DeleteAfter.Time,
                ControlTokenHash = protectedNote.ControlTokenHash,
                Signature = protectedNote.Signature,
                EncryptionKeyOptions = protectedNote.EncryptionKeyOptions,
                SigningKeyOptions = protectedNote.SigningKeyOptions
            },
            _ => new()
            {
                Id = note.Id,
                Content = note.Content,
                DeleteOnReceipt = note.DeleteAfter.Receipt,
                DeleteAfterTime = note.DeleteAfter.Time,
                ControlTokenHash = note.ControlTokenHash,
                Signature = null,
                EncryptionKeyOptions = null,
                SigningKeyOptions = null
            }
        };
    }
}

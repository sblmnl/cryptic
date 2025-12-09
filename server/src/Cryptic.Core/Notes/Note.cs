using Cryptic.Core.Notes.Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StronglyTypedIds;

namespace Cryptic.Core.Notes;

[StronglyTypedId]
public readonly partial struct NoteId { }

public class Note
{
    public NoteId Id { get; } = NoteId.New();
    public required string Content { get; init; }
    public required DeleteAfter DeleteAfter { get; init; }
    public required ControlTokenHash ControlTokenHash { get; init; }
    public PasswordHash? PasswordHash { get; init; }
    public string? ClientMetadata { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    private Note() { }

    public Result VerifyPassword(string? password)
    {
        if (PasswordHash is not null && password is null)
        {
            return Result.Fail(new NotePasswordNotProvidedError(Id));
        }

        if (PasswordHash is not null && password is not null && !PasswordHash.Verify(password))
        {
            return Result.Fail(new IncorrectNotePasswordError(Id));
        }

        return Result.Ok();
    }

    public bool HasDeleteAfterPassed() => DateTime.UtcNow > CreatedAt.Add(DeleteAfter.ToTimeSpan());

    public static Result<Note> Create(
        string content,
        DeleteAfter deleteAfter,
        ControlTokenHash controlTokenHash,
        PasswordHash? passwordHash,
        string? clientMetadata)
    {
        if (string.IsNullOrWhiteSpace(content) || content.Length < 3)
        {
            return Result.Fail(new NoteContentTooShortError());
        }

        if (content.Length > 16_384)
        {
            return Result.Fail(new NoteContentTooLongError());
        }

        if (clientMetadata is not null && clientMetadata.Length > 1_024)
        {
            return Result.Fail(new NoteClientMetadataTooLongError());
        }

        return Result.Ok(new Note
        {
            Content = content,
            DeleteAfter = deleteAfter,
            ControlTokenHash = controlTokenHash,
            PasswordHash = passwordHash,
            ClientMetadata = clientMetadata,
        });
    }
}

internal sealed class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> entityBuilder)
    {
        entityBuilder.HasKey(x => x.Id);

        entityBuilder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new NoteId(x));

        entityBuilder.Property(x => x.Content)
            .HasMaxLength(16_384);

        entityBuilder.Property(x => x.ControlTokenHash)
            .HasConversion(
                x => x.Serialize(),
                x => ControlTokenHash.Deserialize(x));

        entityBuilder.Property(x => x.PasswordHash)
            .HasConversion(
                x => x != null ? x.Serialize() : null,
                x => x != null ? PasswordHash.Deserialize(x) : null);

        entityBuilder.Property(x => x.ClientMetadata)
            .HasMaxLength(1_024);
    }
}

using Cryptic.Core.Notes.Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cryptic.Core.Notes;

[JsonConverter(typeof(NoteIdJsonConverter))]
public readonly record struct NoteId(Guid Value)
{
    public static NoteId NewNoteId() => new(Guid.NewGuid());
}

public class NoteIdJsonConverter : JsonConverter<NoteId>
{
    public override NoteId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        new(reader.GetGuid());

    public override void Write(Utf8JsonWriter writer, NoteId value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}

public class Note
{
    public NoteId Id { get; } = NoteId.NewNoteId();
    public required string Content { get; init; }
    public required DeleteAfter DeleteAfter { get; init; }
    public required ControlTokenHash ControlTokenHash { get; init; }
    public PasswordHash? PasswordHash { get; init; }
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
        PasswordHash? passwordHash)
    {
        if (string.IsNullOrWhiteSpace(content) || content.Length < 3)
        {
            return Result.Fail(new NoteContentTooShortError());
        }

        if (content.Length > 5000)
        {
            return Result.Fail(new NoteContentTooLongError());
        }

        return Result.Ok(new Note
        {
            Content = content,
            DeleteAfter = deleteAfter,
            ControlTokenHash = controlTokenHash,
            PasswordHash = passwordHash,
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
            .HasMaxLength(5000);

        entityBuilder.Property(x => x.ControlTokenHash)
            .HasConversion(
                x => x.Serialize(),
                x => ControlTokenHash.Deserialize(x));

        entityBuilder.Property(x => x.PasswordHash)
            .HasConversion(
                x => x != null ? x.Serialize() : null,
                x => x != null ? PasswordHash.Deserialize(x) : null);
    }
}

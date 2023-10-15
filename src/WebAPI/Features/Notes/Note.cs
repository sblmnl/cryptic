using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebAPI.Features.Notes;

public record NoteId(Guid Value)
{
    public static NoteId New() => new(Guid.NewGuid());

    public override string ToString()
    {
        return Value.ToString();
    }
}

public class Note
{
    public NoteId Id { get; } = NoteId.New();
    public required string Content { get; init; }
    public required DeleteAfter DeleteAfter { get; init; }
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
    
    public bool ShouldBeDeleted() => DeleteAfter != DeleteAfter.Reading
                             && DeleteAfter != DeleteAfter.ReadingNoWarning
                             && DateTimeOffset.UtcNow >= CreatedAt + DeleteAfter.DeleteIn;
}

public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                v => v.Value,
                v => new NoteId(v));

        builder.Property(e => e.DeleteAfter)
            .HasConversion(
                v => v.ToString(),
                v => DeleteAfter.FromShorthand(v));

        builder.Property(e => e.CreatedAt).IsRequired();
    }
}
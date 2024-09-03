namespace Cryptic.Shared.Features.Notes.ReadNote;

public record ReadNoteCommand : IRequest<Result<ReadNoteResponse>>
{
    public required Guid NoteId { get; init; }
    public required string? Password { get; init; }
}

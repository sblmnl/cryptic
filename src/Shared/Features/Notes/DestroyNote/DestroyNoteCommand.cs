namespace Cryptic.Shared.Features.Notes.DestroyNote;

public record DestroyNoteCommand : IRequest<Result<Nothing>>
{
    public required Guid NoteId { get; init; }
    public required string ControlToken { get; init; }
}

namespace Cryptic.Shared.Features.Notes.CreateNote;

public record CreateNoteResponse
{
    public required Domain.Note Note { get; init; }
    public required string ControlToken { get; init; }
}

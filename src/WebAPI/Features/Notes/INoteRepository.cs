namespace WebAPI.Features.Notes;

public interface INoteRepository
{
    public Task AddNoteAsync(Note note, CancellationToken ct);
    public Task<Note?> GetNoteByIdAsync(NoteId noteId, CancellationToken ct);
    public Task RemoveNoteAsync(Note note, CancellationToken ct);
}

using Microsoft.EntityFrameworkCore;

namespace WebAPI.Features.Notes;

public interface INoteRepository
{
    public Task AddNoteAsync(Note note, CancellationToken ct);
    public Task<Note?> GetNoteByIdAsync(NoteId noteId, CancellationToken ct);
    public Task RemoveNoteAsync(Note note, CancellationToken ct);
}

public class NoteRepository : INoteRepository
{
    private readonly IAppDbContext _db;
    
    public NoteRepository(IAppDbContext db)
    {
        _db = db;
    }

    public async Task AddNoteAsync(Note note, CancellationToken ct)
    {
        await _db.Notes.AddAsync(note, ct);
        await _db.SaveChangesAsync(ct);
    }

    public Task<Note?> GetNoteByIdAsync(NoteId noteId, CancellationToken ct)
    {
        return _db.Notes.FirstOrDefaultAsync(x => x.Id == noteId, ct);
    }

    public async Task RemoveNoteAsync(Note note, CancellationToken ct)
    {
        await _db.Notes
            .Where(x => x.Id == note.Id)
            .ExecuteDeleteAsync(ct);
    }
}
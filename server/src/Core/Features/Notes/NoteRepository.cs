using Cryptic.Core.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Cryptic.Core.Features.Notes;

public interface INoteRepository
{
    Task AddNoteAsync(Domain.Note note, CancellationToken ct);
    void RemoveNote(Domain.Note note);
    Task<Domain.Note?> GetNoteByIdAsync(Guid noteId, CancellationToken ct);
}

public class NoteRepository : INoteRepository
{
    private readonly AppDbContext _db;
    
    public NoteRepository(AppDbContext dbContext)
    {
        _db = dbContext;
    }

    public async Task AddNoteAsync(Domain.Note note, CancellationToken ct)
    {
        await _db.Notes.AddAsync(note.ToStorageType(), ct);
    }

    public void RemoveNote(Domain.Note note)
    {
        _db.Notes.Remove(note.ToStorageType());
    }

    public async Task<Domain.Note?> GetNoteByIdAsync(Guid noteId, CancellationToken ct)
    {
        var rawNote = await _db.Notes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == noteId, ct);

        var note = rawNote?.ToDomainType();
    
        return note;
    }
}

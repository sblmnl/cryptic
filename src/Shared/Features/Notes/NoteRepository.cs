using Cryptic.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cryptic.Shared.Features.Notes.Persistence;

public interface INoteRepository
{
    Task AddNoteAsync(Domain.Note note, CancellationToken ct);
    Task RemoveNoteAsync(Domain.Note note, CancellationToken ct);
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
        await _db.SaveChangesAsync(ct);
    }

    public Task RemoveNoteAsync(Domain.Note note, CancellationToken ct)
    {
        return _db.Notes
            .Where(x => x.Id == note.Id)
            .ExecuteDeleteAsync(ct);
    }

    public async Task<Domain.Note?> GetNoteByIdAsync(Guid noteId, CancellationToken ct)
    {
        var rawNote = await _db.Notes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == noteId, ct);

        var note = rawNote?.ToDomainType();
    
        return note;
    }
}

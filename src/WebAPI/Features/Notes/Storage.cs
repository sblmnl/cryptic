using Microsoft.EntityFrameworkCore;

namespace WebAPI.Features.Notes;

public static class Converters
{
    public static Storage.Note ToStorageType(this Domain.Note note)
    {
        DateTimeOffset? deleteAt = note.DeleteAfter switch
        {
            Domain.DeleteAfter.Time x => x.DeleteAt,
            _ => null
        };

        var doNotWarn = note.DeleteAfter switch
        {
            Domain.DeleteAfter.Reading x => x.DoNotWarn,
            _ => false
        };
        
        return new(
            note.Id,
            note.Content,
            doNotWarn,
            deleteAt);
    }

    public static Domain.Note ToDomainType(this Storage.Note note)
    {
        return new(
            note.Id,
            note.Content,
            Domain.DeleteAfter.From(note.DeleteAt, note.DoNotWarn));
    }
}

public static class Storage
{
    public record Note(
        Guid Id,
        string Content,
        bool DoNotWarn,
        DateTimeOffset? DeleteAt);
    
    public class NoteRepository
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
            var note = (await _db.Notes.FirstOrDefaultAsync(x => x.Id == noteId, ct))?.ToDomainType();

            if (note is null)
            {
                return null;
            }

            if (note.DeleteAfter is Domain.DeleteAfter.Time deleteAfterTime
                && deleteAfterTime.DeleteAt < DateTimeOffset.UtcNow)
            {
                await RemoveNoteAsync(note, ct);
                return null;
            }

            return note;
        }
    }
}

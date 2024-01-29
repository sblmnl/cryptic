using System.Runtime.Intrinsics.Arm;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using WebAPI.Common.Security;

namespace WebAPI.Features.Notes;

public static class Storage
{
    public record Note
    {
        public required Guid Id { get; init; }
        public required string Content { get; init; }
        public required bool DoNotWarn { get; init; }
        public required DateTimeOffset? DeleteAt { get; init; }
        public required string ControlTokenHash { get; init; }
    }
    
    public static Result<Domain.Note> ToDomainType(this Note note)
    {
        if (!Pbkdf2Hash.TryParse(note.ControlTokenHash, out var tokenHash)
            || tokenHash is null)
        {
            return Result.Fail("Invalid token hash!");
        }

        return Result.Ok(new Domain.Note
        {
            Id = note.Id,
            Content = note.Content,
            DeleteAfter = Domain.DeleteAfter.From(note.DeleteAt, note.DoNotWarn),
            ControlTokenHash = tokenHash
        });
    }
    
    public static Note ToStorageType(this Domain.Note note)
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
        
        return new()
        {
            Id = note.Id,
            Content = note.Content,
            DoNotWarn = doNotWarn,
            DeleteAt = deleteAt,
            ControlTokenHash = note.ControlTokenHash.ToString()
        };
    }

    public interface INoteRepository
    {
        Task AddNoteAsync(Domain.Note note, CancellationToken ct);
        Task RemoveNoteAsync(Domain.Note note, CancellationToken ct);
        Task<Domain.Note?> GetNoteByIdAsync(Guid noteId, CancellationToken ct);
    }
    
    public class NoteRepository : INoteRepository
    {
        private readonly AppDbContext _db;
        private readonly ILogger _logger;
    
        public NoteRepository(AppDbContext dbContext, ILogger<NoteRepository> logger)
        {
            _db = dbContext;
            _logger = logger;
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
            var storageNote = await _db.Notes.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == noteId, ct);

            if (storageNote is null)
            {
                return null;
            }
            
            var domainNoteResult = storageNote.ToDomainType();
            
            if (domainNoteResult.IsFailed)
            {
                foreach (var error in domainNoteResult.Errors)
                {
                    _logger.LogError(
                        "<{EntityType}> {EntityId}: Contains invalid data! - {ErrorMessage}",
                        typeof(Note),
                        noteId,
                        error.Message);
                }
                
                return null;
            }
            
            if (domainNoteResult.Value.DeleteAfter is Domain.DeleteAfter.Time deleteAfter
                && deleteAfter.DeleteAt < DateTimeOffset.UtcNow)
            {
                await RemoveNoteAsync(domainNoteResult.Value, ct);
                return null;
            }

            return domainNoteResult.Value;
        }
    }
}

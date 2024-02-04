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
        public required string? PasswordHash { get; init; }
        public required string? KeyOptions { get; init; }
    }
    
    public static Result<Domain.Note> ToDomainType(this Note note)
    {
        if (!Pbkdf2.Key.TryParse(note.ControlTokenHash, out var tokenHash)
            || tokenHash is null)
        {
            return Result.Fail("Invalid token hash!");
        }

        if (note.PasswordHash is null
            && note.KeyOptions is null)
        {
            return Result.Ok(new Domain.Note.Unprotected(
                note.Id,
                note.Content,
                Domain.DeleteAfter.From(note.DeleteAt, note.DoNotWarn),
                tokenHash) as Domain.Note);
        }
        
        if (note.PasswordHash is null
            || !Pbkdf2.Key.TryParse(note.PasswordHash, out var passwordHash)
            || passwordHash is null)
        {
            return Result.Fail("Invalid password hash!");
        }
        
        if (note.KeyOptions is null
            || !Pbkdf2.Options.TryParse(note.KeyOptions, out var keyOptions)
            || keyOptions is null)
        {
            return Result.Fail("Invalid key options!");
        }
        
        return Result.Ok(new Domain.Note.Protected(
            note.Id,
            note.Content,
            Domain.DeleteAfter.From(note.DeleteAt, note.DoNotWarn),
            tokenHash,
            passwordHash,
            keyOptions) as Domain.Note);
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
        
        var passwordHash = note switch
        {
            Domain.Note.Protected x => x.PasswordHash,
            _ => null
        };
        
        var keyOptions = note switch
        {
            Domain.Note.Protected x => x.KeyOptions,
            _ => null
        };
        
        return new()
        {
            Id = note.Id,
            Content = note.Content,
            DoNotWarn = doNotWarn,
            DeleteAt = deleteAt,
            ControlTokenHash = note.ControlTokenHash.ToString(),
            PasswordHash = passwordHash?.ToString(),
            KeyOptions = keyOptions?.ToString()
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

using Cryptic.Core.Notes.Errors;
using Microsoft.EntityFrameworkCore;

namespace Cryptic.Core.Notes.Commands;

public class ReadNoteResponse
{
    public required NoteId NoteId { get; init; }
    public required string Content { get; init; }
    public required bool Destroyed { get; init; }
    public string? ClientMetadata { get; init; }
}

public class ReadNoteCommand : ICommand<Result<ReadNoteResponse>>
{
    public required NoteId NoteId { get; init; }
    public string? Password { get; init; }
}

public class ReadNoteCommandHandler : ICommandHandler<ReadNoteCommand, Result<ReadNoteResponse>>
{
    private readonly AppDbContext _db;

    public ReadNoteCommandHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<ReadNoteResponse>> HandleAsync(ReadNoteCommand command, CancellationToken ct = default)
    {
        var note = await _db.Notes.FirstOrDefaultAsync(x => x.Id == command.NoteId, ct);

        if (note is null || note.HasDeleteAfterPassed())
        {
            return Result.Fail(new NoteNotFoundError(command.NoteId));
        }

        var passwordVerificationResult = note.VerifyPassword(command.Password);

        if (passwordVerificationResult.IsFailed)
        {
            return passwordVerificationResult.ToResult<ReadNoteResponse>();
        }

        if (note.DeleteAfter == DeleteAfter.Viewing)
        {
            _db.Notes.Remove(note);
            await _db.SaveChangesAsync(ct);
        }

        return Result.Ok(new ReadNoteResponse
        {
            NoteId = note.Id,
            Content = note.Content,
            Destroyed = note.DeleteAfter == DeleteAfter.Viewing,
            ClientMetadata = note.ClientMetadata,
        });
    }
}

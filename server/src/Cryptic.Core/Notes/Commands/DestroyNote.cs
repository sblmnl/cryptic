using Cryptic.Core.Notes.Errors;
using Microsoft.EntityFrameworkCore;

namespace Cryptic.Core.Notes.Commands;

public class DestroyNoteCommand : ICommand<Result>
{
    public required NoteId NoteId { get; init; }
    public required ControlToken ControlToken { get; init; }
}

public class DestroyNoteCommandHandler : ICommandHandler<DestroyNoteCommand, Result>
{
    private readonly AppDbContext _db;

    public DestroyNoteCommandHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Result> HandleAsync(DestroyNoteCommand request, CancellationToken cancellationToken = default)
    {
        var note = await _db.Notes.FirstOrDefaultAsync(x => x.Id == request.NoteId, cancellationToken);

        if (note is null || note.HasDeleteAfterPassed())
        {
            return Result.Fail(new NoteNotFoundError(request.NoteId));
        }

        if (!note.ControlTokenHash.Verify(request.ControlToken))
        {
            return Result.Fail(new IncorrectNoteControlTokenError(note.Id));
        }

        _db.Notes.Remove(note);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}

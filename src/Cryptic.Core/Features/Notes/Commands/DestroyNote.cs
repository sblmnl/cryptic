using Cryptic.Core.Persistence;

namespace Cryptic.Core.Features.Notes.Commands;

public record DestroyNoteCommand : IRequest<Result<Nothing>>
{
    public required Guid NoteId { get; init; }
    public required string ControlToken { get; init; }
}

public static class DestroyNoteErrors
{
    public static readonly CodedError NoteNotFound = new()
    {
        Code = "Cryptic.Notes.DestroyNote.NoteNotFound",
        Message = "That note doesn't exist!"
    };
    
    public static readonly CodedError IncorrectControlToken = new()
    {
        Code = "Cryptic.Notes.DestroyNote.IncorrectControlToken",
        Message = "The provided control token was incorrect!"
    };
}

public class DestroyNoteCommandHandler : IRequestHandler<DestroyNoteCommand, Result<Nothing>>
{
    private readonly INoteRepository _noteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DestroyNoteCommandHandler(INoteRepository noteRepository, IUnitOfWork unitOfWork)
    {
        _noteRepository = noteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Nothing>> Handle(DestroyNoteCommand command, CancellationToken ct)
    {
        var note = await _noteRepository.GetNoteByIdAsync(command.NoteId, ct);

        if (note is null)
        {
            return new Result<Nothing>.Fail(DestroyNoteErrors.NoteNotFound);
        }

        if (!ControlToken.TryParse(command.ControlToken, out var controlToken)
            || controlToken is null
            || !note.ControlTokenHash.Verify(controlToken))
        {
            return new Result<Nothing>.Fail(DestroyNoteErrors.IncorrectControlToken);
        }
        
        _noteRepository.RemoveNote(note);
        await _unitOfWork.SaveChangesAsync(ct);

        return new Result<Nothing>.Ok(new());
    }
}

using System.Text;
using Cryptic.Shared.Persistence;

namespace Cryptic.Shared.Features.Notes.DestroyNote;

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
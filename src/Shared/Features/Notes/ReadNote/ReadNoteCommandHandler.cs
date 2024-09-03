using Cryptic.Shared.Persistence;

namespace Cryptic.Shared.Features.Notes.ReadNote;

public class ReadNoteCommandHandler : IRequestHandler<ReadNoteCommand, Result<ReadNoteResponse>>
{
    private readonly INoteRepository _noteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReadNoteCommandHandler(INoteRepository noteRepository, IUnitOfWork unitOfWork)
    {
        _noteRepository = noteRepository;
        _unitOfWork = unitOfWork;
    }

    private static Result<ReadNoteResponse> ReadEncryptedNote(Domain.NoteContent.Encrypted noteContent, string password)
    {
        var result = noteContent.Decrypt(password);

        return result switch
        {
            Result<Domain.NoteContent.Plaintext>.Ok successResult =>
                new Result<ReadNoteResponse>.Ok(new ReadNoteResponse(successResult.Value)),
            _ => new Result<ReadNoteResponse>.Fail(ReadNoteErrors.IncorrectPassword)
        };
    }

    private async Task DestroyNote(Domain.Note note, CancellationToken ct)
    {
        _noteRepository.RemoveNote(note);
        await _unitOfWork.SaveChangesAsync(ct);
    }
    
    public async Task<Result<ReadNoteResponse>> Handle(ReadNoteCommand command, CancellationToken ct)
    {
        var note = await _noteRepository.GetNoteByIdAsync(command.NoteId, ct);
        
        if (note is null)
        {
            return new Result<ReadNoteResponse>.Fail(ReadNoteErrors.NoteNotFound);
        }

        if (note.HasDeleteAfterTimePassed())
        {
            await DestroyNote(note, ct);
            
            return new Result<ReadNoteResponse>.Fail(ReadNoteErrors.NoteNotFound);
        }

        var result = note.Content switch
        {
            Domain.NoteContent.Encrypted encryptedContent =>
                ReadEncryptedNote(encryptedContent, command.Password ?? ""),
            _ => new Result<ReadNoteResponse>.Ok(new(note.Content))
        };

        if (note.DeleteAfter.Receipt)
        {
            await DestroyNote(note, ct);
        }

        return result;
    }
}
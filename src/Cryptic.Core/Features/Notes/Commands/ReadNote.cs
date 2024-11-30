using Cryptic.Core.Persistence;

namespace Cryptic.Core.Features.Notes.Commands;

public record ReadNoteResponse(string Content);

public record ReadNoteCommand : IRequest<Result<ReadNoteResponse>>
{
    public required Guid NoteId { get; init; }
    public required string? Password { get; init; }
}

public static class ReadNoteErrors
{
    public static readonly CodedError NoteNotFound = new()
    {
        Code = "Cryptic.Notes.ReadNote.NoteNotFound",
        Message = "That note doesn't exist!"
    };

    public static readonly CodedError IncorrectPassword = new()
    {
        Code = "Cryptic.Notes.ReadNote.IncorrectPassword",
        Message = "The password you entered was incorrect!"
    };
}

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

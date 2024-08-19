using Cryptic.Shared.Features.Notes.Persistence;

namespace Cryptic.Shared.Features.Notes.CreateNote;

public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, Result<CreateNoteResponse>>
{
    private readonly INoteRepository _noteRepository;
    
    public CreateNoteCommandHandler(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    public async Task<Result<CreateNoteResponse>> Handle(CreateNoteCommand command, CancellationToken ct)
    {
        var deleteAfter = new DeleteAfter
        {
            Receipt = command.DeleteOnReceipt,
            Time = command.DeleteAfterTime
        };
        
        if (deleteAfter.Time <= DateTimeOffset.UtcNow)
        {
            return new Result<CreateNoteResponse>.Failure(CreateNoteErrors.DeleteAfterAlreadyPassed);
        }

        var controlToken = ControlToken.Create();
        var controlTokenHash = Pbkdf2.Key.Create(controlToken.Value);

        var unprotectedNote = new Domain.Note.Unprotected
        {
            Id = Guid.NewGuid(),
            Content = command.Content,
            DeleteAfter = deleteAfter,
            ControlTokenHash = controlTokenHash
        };
        
        Domain.Note note = command.Password is not null
            ? unprotectedNote.Encrypt(command.Password)
            : unprotectedNote;

        await _noteRepository.AddNoteAsync(note, ct);

        return new Result<CreateNoteResponse>.Success(new()
        {
            Note = note,
            ControlToken = controlToken.ToString()
        });
    }
}
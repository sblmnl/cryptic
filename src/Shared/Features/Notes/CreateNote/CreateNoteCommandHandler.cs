using Cryptic.Shared.Persistence;

namespace Cryptic.Shared.Features.Notes.CreateNote;

public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, Result<CreateNoteResponse>>
{
    private readonly INoteRepository _noteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateNoteCommandHandler(INoteRepository noteRepository, IUnitOfWork unitOfWork)
    {
        _noteRepository = noteRepository;
        _unitOfWork = unitOfWork;
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
            return new Result<CreateNoteResponse>.Fail(CreateNoteErrors.DeleteAfterAlreadyPassed);
        }

        var controlToken = ControlToken.Create();
        var controlTokenHash = Pbkdf2.Key.Create(controlToken.Value);

        Domain.NoteContent noteContent = command.Password is null
            ? new Domain.NoteContent.Plaintext { Value = command.Content }
            : Domain.NoteContent.Encrypted.Create(command.Content, command.Password);

        var note = new Domain.Note
        {
            Id = Guid.NewGuid(),
            Content = noteContent,
            DeleteAfter = deleteAfter,
            ControlTokenHash = controlTokenHash
        };

        await _noteRepository.AddNoteAsync(note, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new Result<CreateNoteResponse>.Ok(new()
        {
            Note = note,
            ControlToken = controlToken.ToString()
        });
    }
}
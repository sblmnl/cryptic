using Cryptic.Core.Persistence;

namespace Cryptic.Core.Features.Notes.Commands;

public record CreateNoteResponse
{
    public required Domain.Note Note { get; init; }
    public required string ControlToken { get; init; }
}

public record CreateNoteCommand : IRequest<Result<CreateNoteResponse>>
{
    public required string Content { get; init; }
    public required DateTimeOffset DeleteAfterTime { get; init; }
    public required bool DeleteOnReceipt { get; init; }
    public required string? Password { get; init; }
}

public static class CreateNoteErrors
{
    public static readonly CodedError DeleteAfterAlreadyPassed = new()
    {
        Code = "Cryptic.Notes.CreateNote.DeleteAfterAlreadyPassed",
        Message = "The provided date and time to delete the note has already passed!"
    };
}

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
        var controlTokenHash = Pbkdf2.Key.Create(controlToken);

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

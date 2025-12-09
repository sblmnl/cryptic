namespace Cryptic.Core.Notes.Commands;

public class CreateNoteResponse
{
    public required NoteId NoteId { get; init; }
    public required ControlToken ControlToken { get; init; }
}

public class CreateNoteCommand : ICommand<Result<CreateNoteResponse>>
{
    public required string Content { get; init; }
    public DeleteAfter DeleteAfter { get; init; }
    public string? Password { get; init; }
    public string? ClientMetadata { get; init; }
}

public class CreateNoteCommandHandler : ICommandHandler<CreateNoteCommand, Result<CreateNoteResponse>>
{
    private readonly AppDbContext _db;

    public CreateNoteCommandHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<CreateNoteResponse>> HandleAsync(
        CreateNoteCommand message,
        CancellationToken cancellationToken = default)
    {
        var controlToken = ControlToken.Create();
        var controlTokenHash = ControlTokenHash.Create(controlToken);
        var passwordHash = message.Password is not null ? PasswordHash.Create(message.Password) : null;

        var result = Note.Create(
            message.Content,
            message.DeleteAfter,
            controlTokenHash,
            passwordHash,
            message.ClientMetadata);

        if (result.IsFailed)
        {
            return result.ToResult<CreateNoteResponse>();
        }

        _db.Notes.Add(result.Value);

        await _db.SaveChangesAsync(cancellationToken);

        return result.Map(x => new CreateNoteResponse
        {
            NoteId = x.Id,
            ControlToken = controlToken,
        });
    }
}

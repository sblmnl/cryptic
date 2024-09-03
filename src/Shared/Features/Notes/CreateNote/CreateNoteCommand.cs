namespace Cryptic.Shared.Features.Notes.CreateNote;

public record CreateNoteCommand : IRequest<Result<CreateNoteResponse>>
{
    public required string Content { get; init; }
    public required DateTimeOffset DeleteAfterTime { get; init; }
    public required bool DeleteOnReceipt { get; init; }
    public required string? Password { get; init; }
}

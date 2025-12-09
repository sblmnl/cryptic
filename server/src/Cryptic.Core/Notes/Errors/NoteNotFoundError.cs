namespace Cryptic.Core.Notes.Errors;

public sealed class NoteNotFoundError : ResourceNotFoundError<string>
{
    public const string ErrorCode = "NoteNotFound";
    public const string ErrorMessage = "Note not found!";

    public NoteNotFoundError(NoteId noteId) : base(ErrorCode, ErrorMessage, "NoteId", noteId.ToString()) { }

    public NoteNotFoundError(string noteId) : base(ErrorCode, ErrorMessage, "NoteId", noteId) { }
}

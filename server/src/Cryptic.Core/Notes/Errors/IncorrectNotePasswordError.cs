namespace Cryptic.Core.Notes.Errors;

public class IncorrectNotePasswordError : IncorrectPasswordError<NoteId>
{
    public const string ErrorCode = "IncorrectNotePassword";
    public const string ErrorMessage = "Incorrect password!";

    public IncorrectNotePasswordError(NoteId noteId) : base(ErrorCode, ErrorMessage, "NoteId", noteId) { }
}

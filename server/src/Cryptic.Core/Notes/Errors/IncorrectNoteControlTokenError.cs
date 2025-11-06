namespace Cryptic.Core.Notes.Errors;

public class IncorrectNoteControlTokenError : IncorrectControlTokenError<NoteId>
{
    public const string ErrorCode = "IncorrectNoteControlToken";
    public const string ErrorMessage = "Incorrect control token!";

    public IncorrectNoteControlTokenError(NoteId noteId) : base(ErrorCode, ErrorMessage, "NoteId", noteId) { }
}

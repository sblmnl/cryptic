namespace Cryptic.Core.Notes.Errors;

public class NoteContentTooShortError : CodedError
{
    public const string ErrorCode = "NoteContentTooShort";
    public const string ErrorMessage = "Note content must be at least 3 characters!";

    public NoteContentTooShortError() : base(ErrorCode, ErrorMessage) { }
}

namespace Cryptic.Core.Notes.Errors;

public class NoteContentTooLongError : CodedError
{
    public const string ErrorCode = "NoteContentTooLong";
    public const string ErrorMessage = "Note content cannot exceed 16,384 characters!";

    public NoteContentTooLongError() : base(ErrorCode, ErrorMessage) { }
}

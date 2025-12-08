namespace Cryptic.Core.Notes.Errors;

public class NoteClientMetadataTooLongError : CodedError
{
    public const string ErrorCode = "NoteClientMetadataTooLong";
    public const string ErrorMessage = "Note client metadata cannot exceed 1,024 characters!";

    public NoteClientMetadataTooLongError() : base(ErrorCode, ErrorMessage) { }
}

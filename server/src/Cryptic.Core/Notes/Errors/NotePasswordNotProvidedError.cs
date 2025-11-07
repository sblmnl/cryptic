namespace Cryptic.Core.Notes.Errors;

public class NotePasswordNotProvidedError : PasswordNotProvidedError<NoteId>
{
    public const string ErrorCode = "NotePasswordNotProvided";
    public const string ErrorMessage = "Password not provided!";

    public NotePasswordNotProvidedError(NoteId noteId) : base(ErrorCode, ErrorMessage, "NoteId", noteId) { }
}

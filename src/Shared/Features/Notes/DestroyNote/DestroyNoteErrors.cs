namespace Cryptic.Shared.Features.Notes.DestroyNote;

public static class DestroyNoteErrors
{
    public static readonly CodedError NoteNotFound = new()
    {
        Code = "Cryptic.Notes.DestroyNote.NoteNotFound",
        Message = "That note doesn't exist!"
    };
    
    public static readonly CodedError IncorrectControlToken = new()
    {
        Code = "Cryptic.Notes.DestroyNote.IncorrectControlToken",
        Message = "The provided control token was incorrect!"
    };
}
namespace Cryptic.Shared.Features.Notes.ReadNote;

public static class ReadNoteErrors
{
    public static readonly CodedError NoteNotFound = new()
    {
        Code = "Cryptic.Notes.ReadNote.NoteNotFound",
        Message = "That note doesn't exist!"
    };

    public static readonly CodedError IncorrectPassword = new()
    {
        Code = "Cryptic.Notes.ReadNote.IncorrectPassword",
        Message = "The password you entered was incorrect!"
    };
}
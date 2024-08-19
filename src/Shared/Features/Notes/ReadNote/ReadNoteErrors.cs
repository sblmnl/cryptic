namespace Cryptic.Shared.Features.Notes.ReadNote;

public static class ReadNoteErrors
{
    public static readonly Error NoteNotFound = new(
        "Cryptic.Notes.ReadNote.NoteNotFound",
        "That note doesn't exist!");
    
    public static readonly Error IncorrectPassword = new(
        "Cryptic.Notes.ReadNote.IncorrectPassword",
        "The password you entered was incorrect!");
}
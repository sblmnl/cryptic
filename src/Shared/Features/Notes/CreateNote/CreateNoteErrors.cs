namespace Cryptic.Shared.Features.Notes.CreateNote;

public static class CreateNoteErrors
{
    public static readonly Error DeleteAfterAlreadyPassed = new(
        "Cryptic.Notes.CreateNote.DeleteAfterAlreadyPassed",
        "The provided date and time to delete the note has already passed!");
}
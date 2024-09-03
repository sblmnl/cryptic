namespace Cryptic.Shared.Features.Notes.CreateNote;

public static class CreateNoteErrors
{
    public static readonly CodedError DeleteAfterAlreadyPassed = new()
    {
        Code = "Cryptic.Notes.CreateNote.DeleteAfterAlreadyPassed",
        Message = "The provided date and time to delete the note has already passed!"
    };
}
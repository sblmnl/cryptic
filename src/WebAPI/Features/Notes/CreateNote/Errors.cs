using WebAPI.Common;

namespace WebAPI.Features.Notes.CreateNote;

public static class Errors
{
    public static readonly Error DeleteAfterAlreadyPassed = new(
        "Notes.CreateNote.DeleteAfterAlreadyPassed",
        "The provided date and time to delete the note has already passed!");
}

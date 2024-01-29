using WebAPI.Common;

namespace WebAPI.Features.Notes.ReadNote;

public static class Errors
{
    public static readonly Error DeleteAfterReadingWarningNotAcknowledged =
        new Error(
            "Notes.ReadNote.DeleteAfterReadingWarningNotAcknowledged", 
            "This note is set to delete after reading it. In order to read this note, you must add the " +
            "'acknowledge' query parameter to the request.");
    
    public static readonly Error NoteNotFound =
        new Error(
            "Notes.ReadNote.NoteNotFound",
            "That note doesn't exist!");
}

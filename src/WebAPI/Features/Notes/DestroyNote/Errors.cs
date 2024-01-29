using WebAPI.Common;

namespace WebAPI.Features.Notes.DestroyNote;

public static class Errors
{
    public static readonly Error NoteNotFound =
        new Error(
            "Notes.DestroyNote.NoteNotFound",
            "That note doesn't exist!");
}
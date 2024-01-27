namespace WebAPI.Features.Notes.ReadNote;

public static class HttpErrors
{
    public static readonly IResult DeleteAfterReadingWarningNotAcknowledged = Results.BadRequest(
        "This note is set to delete after reading it. In order to read this note, you must add the " +
        "'acknowledge' query parameter to the request.");
}

public static class HttpHandler
{
    public static readonly Delegate Handler = async (
        Guid noteId,
        Storage.NoteRepository noteRepository,
        HttpContext ctx) =>
    {
        var note = await noteRepository.GetNoteByIdAsync(noteId, ctx.RequestAborted);
        
        if (note is null)
        {
            return Results.NotFound();
        }

        if (note.DeleteAfter is Domain.DeleteAfter.Reading { DoNotWarn: false }
            && !ctx.Request.Query.ContainsKey("acknowledge"))
        {
            return HttpErrors.DeleteAfterReadingWarningNotAcknowledged;
        }

        if (note.DeleteAfter is Domain.DeleteAfter.Reading)
        {
            await noteRepository.RemoveNoteAsync(note, ctx.RequestAborted);
        }
        
        return Results.Ok(note.Content);
    };
}

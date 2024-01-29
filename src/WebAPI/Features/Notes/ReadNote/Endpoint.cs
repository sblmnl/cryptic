using WebAPI.Common;

namespace WebAPI.Features.Notes.ReadNote;

public static class Endpoint
{
    public static void MapReadNoteEndpoint(this WebApplication app)
    {
        app.MapGet("/api/notes/{id:guid}", async (
            Guid id,
            Storage.NoteRepository noteRepository,
            HttpContext ctx) =>
        {
            var note = await noteRepository.GetNoteByIdAsync(id, ctx.RequestAborted);

            if (note is null)
            {
                return Results.NotFound(new HttpResponseBody(Errors.NoteNotFound));
            }

            if (note.DeleteAfter is Domain.DeleteAfter.Reading { DoNotWarn: false }
                && !ctx.Request.Query.ContainsKey("acknowledge"))
            {
                return Results.BadRequest(new HttpResponseBody(
                    Errors.DeleteAfterReadingWarningNotAcknowledged,
                    new List<Link>
                    {
                        new Link(
                            ctx.Request.GetBaseUri().WithQueryParam("acknowledge").ToString(),
                            "self",
                            HttpMethods.Get)
                    }));
            }

            if (note.DeleteAfter is Domain.DeleteAfter.Reading)
            {
                await noteRepository.RemoveNoteAsync(note, ctx.RequestAborted);
            }

            return Results.Ok(new HttpResponseBody(note.Content));
        }).WithName("ReadNote").WithOpenApi();
    }
}

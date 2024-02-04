using System.Text;
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
            
            if (note is Domain.Note.Protected protectedNote)
            {
                if (!ctx.Request.TryGetAuthorization(out var authorization)
                    || !protectedNote.PasswordHash.Verify(authorization!, Encoding.UTF8))
                {
                    return Results.Unauthorized();
                }

                if (!protectedNote.TryDecrypt(authorization!, out var decryptedNote))
                {
                    return Results.StatusCode(500);
                }
                
                note = decryptedNote!;
            }

            if (note.DeleteAfter is Domain.DeleteAfter.Reading)
            {
                await noteRepository.RemoveNoteAsync(note, ctx.RequestAborted);
            }
            
            return Results.Ok(new HttpResponseBody(note.Content));
        })
            .WithName("ReadNote")
            .WithOpenApi();
    }
}

using WebAPI.Common;

namespace WebAPI.Features.Notes.DestroyNote;

public static class Endpoint
{
    public static void MapDestroyNoteEndpoint(this WebApplication app)
    {
        app.MapDelete("/api/notes/{id:guid}", async (
            Guid id,
            Storage.NoteRepository noteRepository,
            HttpContext ctx) =>
        {
            var note = await noteRepository.GetNoteByIdAsync(id, ctx.RequestAborted);

            if (note is null)
            {
                return Results.NotFound(new HttpResponseBody(Errors.NoteNotFound));
            }
            
            if (!ctx.Request.TryGetAuthorization(out var authorization)
                || !Domain.ControlToken.TryParse(authorization!, out var controlToken)
                || !note.ControlTokenHash.Matches(controlToken!.Value))
            {
                return Results.Unauthorized();
            }

            await noteRepository.RemoveNoteAsync(note, ctx.RequestAborted);
            
            return Results.Ok();
        })
            .WithName("DestroyNote")
            .WithOpenApi();
    }
}

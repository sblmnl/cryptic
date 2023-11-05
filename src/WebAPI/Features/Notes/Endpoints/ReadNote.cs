using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Features.Notes.Endpoints;

public static class ReadNote
{
    public class Endpoint : IEndpoint
    {
        public void Map(WebApplication app)
        {
            app.MapGet(
                "/notes/{noteId:guid}", 
                async (
                    [FromServices] INoteRepository noteRepository,
                    Guid noteId,
                    HttpContext ctx) =>
                {
                    var note = await noteRepository.GetNoteByIdAsync(new NoteId(noteId), ctx.RequestAborted);

                    if (note is null)
                    {
                        return Results.NotFound("That note doesn't exist!");
                    }
                    
                    if (note.ShouldBeDeleted())
                    {
                        await noteRepository.RemoveNoteAsync(note, ctx.RequestAborted);
                        return Results.NotFound("That note doesn't exist!");
                    }

                    if (note.DeleteAfter == DeleteAfter.Reading
                        && !ctx.Request.Query.ContainsKey("acknowledge"))
                    {
                        return Results.BadRequest(
                            "This note is set to delete after reading it. " + 
                            "In order to read this note, you must add the \"acknowledge\" query parameter to the request.");
                    }

                    if (note.DeleteAfter == DeleteAfter.Reading
                        || note.DeleteAfter == DeleteAfter.ReadingNoWarning)
                    {
                        await noteRepository.RemoveNoteAsync(note, ctx.RequestAborted);
                    }
                    
                    return Results.Ok(note.Content);
                });
        }
    }
}

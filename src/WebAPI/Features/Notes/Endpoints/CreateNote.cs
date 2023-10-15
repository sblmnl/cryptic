using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Features.Notes.Endpoints;

public static class CreateNote
{
    public class Request
    {
        public required string Content { get; init; }
        public required string DeleteAfter { get; init; }
    }
    
    public class Endpoint : IEndpoint
    {
        public void Map(WebApplication app)
        {
            app.MapPost(
                "/notes", 
                async (
                    [FromServices] INoteRepository noteRepository,
                    Request req,
                    HttpContext ctx) =>
                {
                    if (!DeleteAfter.TryFromShorthand(req.DeleteAfter, out var deleteAfter)
                        || deleteAfter is null)
                    {
                        return Results.BadRequest("Unrecognized delete after shorthand!");
                    }
                    
                    var note = new Note
                    {
                        Content = req.Content,
                        DeleteAfter = deleteAfter
                    };
                    
                    await noteRepository.AddNoteAsync(note, ctx.RequestAborted);
                    
                    return Results.Ok(note.Id.ToString());
                });
        }
    }
}

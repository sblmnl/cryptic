using WebAPI.Common;
using WebAPI.Common.Security;

namespace WebAPI.Features.Notes.CreateNote;

public record Request(string Content, DateTimeOffset? DeleteAt, bool DoNotWarn, string? Password);

public record Response()
{
    public required Guid Id { get; init; }
    public required DateTimeOffset? DeleteAt { get; init; }
    public required bool DoNotWarn { get; init; }
    public required string ControlToken { get; init; }
}

public static class Endpoint
{
    public static void MapCreateNoteEndpoint(this WebApplication app)
    {
        app.MapPost("/api/notes", async (
            Request req,
            Storage.NoteRepository noteRepository,
            HttpContext ctx) =>
        {
            var deleteAfter = Domain.DeleteAfter.From(req.DeleteAt, req.DoNotWarn);

            if (deleteAfter is Domain.DeleteAfter.Time deleteAfterTime
                && deleteAfterTime.DeleteAt <= DateTimeOffset.UtcNow)
            {
                return Results.BadRequest(new HttpResponseBody(Errors.DeleteAfterAlreadyPassed));
            }

            var controlToken = Domain.ControlToken.New();

            var note = new Domain.Note.Unprotected(
                Guid.NewGuid(),
                req.Content,
                deleteAfter,
                Pbkdf2.Key.Create(controlToken.Value)) as Domain.Note;
            
            if (req.Password is not null)
            {
                note = Domain.Note.Protected.New((note as Domain.Note.Unprotected)!, req.Password);
            }

            await noteRepository.AddNoteAsync(note, ctx.RequestAborted);

            var res = new Response()
            {
                Id = note.Id,
                DeleteAt = req.DeleteAt,
                DoNotWarn = req.DoNotWarn,
                ControlToken = controlToken.ToString()
            };

            var links = new List<Link>()
            {
                new Link(
                    ctx.Request.GetBaseUri()
                    + (note.DeleteAfter is Domain.DeleteAfter.Reading { DoNotWarn: false }
                        ? $"/{note.Id}?acknowledge="
                        : $"/{note.Id}"),
                    "read-note",
                    HttpMethods.Get),
                new Link(
                    ctx.Request.GetBaseUri().ToString(),
                    "destroy-note",
                    HttpMethods.Delete)
            };
            
            return Results.Created(
                new Uri(note.Id.ToString(), UriKind.Relative),
                new HttpResponseBody(res, links));
        })
            .WithName("CreateNote")
            .WithOpenApi();
    }
}

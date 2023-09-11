using Dapper;

namespace Cryptic.WebAPI.Features.Notes;

public static class CreateNote
{
    public record Request(
        string Content,
        DeleteAfter DeleteAfter);

    public static void Map(
        WebApplication app,
        string dbConnectionString)
    {
        app.MapPost("/notes", async (HttpContext httpContext, Request request) =>
        {
            var note = Note.New(request.Content, request.DeleteAfter);

            using var db = await DbConnectionFactory.NewConnectionAsync(dbConnectionString, httpContext.RequestAborted);

            await db.ExecuteAsync(
                "INSERT INTO notes (id, content, delete_after, created_at) " + 
                "VALUES (@Id, @Content, @DeleteAfter, @CreatedAt);",
                note);

            return Results.Ok(note.Id);
        });
    }
}

using Dapper;

namespace Cryptic.WebAPI.Features.Notes;

public static class CreateNote
{
    public record Request(
        string Content,
        string DeleteAfter);

    public static void Map(
        WebApplication app,
        DbConnectionFactory dbConnectionFactory)
    {
        app.MapPost("/notes", async (Request request) =>
        {
            if (!DeleteAfter.TryFromShorthand(request.DeleteAfter, out var deleteAfter))
            {
                return Results.BadRequest($"Unknown {nameof(DeleteAfter)} shorthand!");
            }

            var note = Note.New(request.Content, deleteAfter);

            using var db = await dbConnectionFactory.NewConnectionAsync();

            await db.ExecuteAsync(
                "INSERT INTO notes (id, content, delete_after, created_at) " + 
                "VALUES (@Id, @Content, @DeleteAfter, @CreatedAt);",
                new {
                    Id = note.Id.Value,
                    Content = note.Content,
                    DeleteAfter = note.DeleteAfter.Shorthand,
                    CreatedAt = note.CreatedAt
                });

            return Results.Ok(note.Id.Value);
        });
    }
}
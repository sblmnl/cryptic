using Dapper;

namespace Cryptic.WebAPI.Features.Notes;

public static class ReadNote
{
    public static void Map(
        WebApplication app,
        string dbConnectionString)
    {
        app.MapGet("/notes/{noteId}", async (HttpContext httpContext, Guid noteId) =>
        {
            using var db = await DbConnectionFactory.NewConnectionAsync(dbConnectionString, httpContext.RequestAborted);

            var note = await db.QueryFirstOrDefaultAsync<Note>(
                "SELECT * FROM notes WHERE Id = @Id",
                new
                {
                    Id = noteId
                });

            if (note is null)
            {
                return Results.NotFound("That note doesn't exist!");
            }
            if (note.IsToBeDeleted)
            {
                await db.ExecuteAsync(
                    "DELETE FROM notes WHERE Id = @Id",
                    new
                    {
                        Id = noteId
                    });

                return Results.NotFound("That note doesn't exist!");
            }

            if (note.DeleteAfterReading)
            {
                await db.ExecuteAsync(
                    "DELETE FROM notes WHERE Id = @Id",
                    new
                    {
                        Id = noteId
                    });
            }

            return Results.Ok(note);
        });
    }
}

using Cryptic.WebAPI.Features.Notes;

namespace Cryptic.WebAPI;

public static class Mapping
{
    public static void MapEndpoints(
        this WebApplication app,
        string dbConnectionStringId = "Postgres")
    {
        var dbConnectionString = app.Configuration.GetConnectionString(dbConnectionStringId)
            ?? throw new ApplicationException($"Connection string: \"{dbConnectionStringId}\" not set!");
            
        var dbConnectionFactory = new DbConnectionFactory(dbConnectionString);

        CreateNote.Map(app, dbConnectionFactory);
    }
}
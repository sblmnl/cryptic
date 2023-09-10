using Cryptic.WebAPI.Features.Notes;

namespace Cryptic.WebAPI;

public static class Routing
{
    public static void MapRoutes(
        this WebApplication app,
        string dbConnectionString)
    {
        CreateNote.Map(app, dbConnectionString);
    }
}
using Cryptic.Web.Server.Notes;

namespace Cryptic.Web.Server;

public static class Setup
{
    public static void MapHttpEndpoints(this WebApplication app)
    {
        app.MapNotesHttpEndpoints();
    }
}

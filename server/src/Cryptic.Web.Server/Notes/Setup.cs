using Cryptic.Web.Server.Notes.Endpoints;

namespace Cryptic.Web.Server.Notes;

public static partial class Setup
{
    public static void MapNotesHttpEndpoints(this WebApplication app)
    {
        app.MapCreateNoteHttpEndpoint();
        app.MapDestroyNoteHttpEndpoint();
        app.MapReadNoteHttpEndpoint();
    }
}

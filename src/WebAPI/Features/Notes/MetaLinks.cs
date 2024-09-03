using Cryptic.Shared.Features.Notes;

namespace Cryptic.WebAPI.Features.Notes;

public static class MetaLinks
{
    private static string GetNoteBaseUri(HttpRequest req, Domain.Note note) => req.GetBaseUri() + $"/{note.Id}";
    
    public static MetaLink GetReadNoteLink(this Domain.Note note, HttpRequest req)
    {
        return new(GetNoteBaseUri(req, note), "read-note", HttpMethods.Get);
    }
    
    public static MetaLink GetDestroyNoteLink(this Domain.Note note, HttpRequest req)
    {
        return new(GetNoteBaseUri(req, note), "destroy-note", HttpMethods.Delete);
    }
}

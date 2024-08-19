using Cryptic.Shared.Features.Notes;

namespace Cryptic.WebAPI.Features.Notes;

public static class MetaLinks
{
    public static MetaLink GetReadNoteLink(this Domain.Note note, HttpRequest req)
    {
        return new(req.GetBaseUri() + $"/{note.Id}", "read-note", HttpMethods.Get);
    }
    
    public static MetaLink GetDestroyNoteLink(this Domain.Note note, HttpRequest req)
    {
        return new(req.GetBaseUri() + $"/{note.Id}", "destroy-note", HttpMethods.Delete);
    }
}

namespace Cryptic.WebAPI.Common.Http;

public class MetaInformation
{
    public IReadOnlyCollection<MetaLink> Links { get; init; } = new List<MetaLink>();
}

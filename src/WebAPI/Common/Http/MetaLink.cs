namespace Cryptic.WebAPI.Common.Http;

public record MetaLink
{
    public string Href { get; init; }
    public string Rel { get; init; }
    public string Method { get; init; }

    public MetaLink(string href, string rel, string method)
    {
        Href = href;
        Rel = rel;
        Method = method;
    }
    
    public MetaLink(Uri href, string rel, string method)
    {
        Href = href.ToString();
        Rel = rel;
        Method = method;
    }
}

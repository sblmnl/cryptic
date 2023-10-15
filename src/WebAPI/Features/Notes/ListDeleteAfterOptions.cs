namespace WebAPI.Features.Notes;

public static class ListDeleteAfterOptions
{
    public class Endpoint : IEndpoint
    {
        public void Map(WebApplication app)
        {
            app.MapGet(
                "/notes/delete-after",
                () =>
                {
                    return Results.Ok(DeleteAfter.GetOptions()
                        .Select(x => x.Shorthand));
                });
        }
    }
}
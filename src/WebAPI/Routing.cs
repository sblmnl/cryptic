namespace WebAPI;

public static class Routing
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = typeof(IEndpoint).Assembly.ExportedTypes
            .Where(type => typeof(IEndpoint).IsAssignableFrom(type)
                           && !type.IsInterface
                           && !type.IsAbstract)
            .Select(Activator.CreateInstance).Cast<IEndpoint>();

        foreach (var endpoint in endpoints)
        {
            endpoint.Map(app);
        }
    }
}
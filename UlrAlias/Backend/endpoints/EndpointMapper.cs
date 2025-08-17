using Ulr_Alias.Backend;

namespace UrlAlias.Extensions;

public static class EndpointMapper
{
    public static void MapAliasEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api");

        app.MapGet("uri/{alias}", ApLogic.HandleAliasRedirect);
        
        group.MapGet("/aliases/{alias}", ApLogic.GetAlias)
            .WithOpenApi();

        group.MapPost("/aliases", ApLogic.PostAlias)
            .WithOpenApi();
    }
}

using UlrAlias.Backend.endpoints;
using UrlAlias.Dtos;

namespace UrlAlias.Extensions;

public static class EndpointMapper
{
    public static void MapAliasEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api");

        app.MapGet("uri/{alias}", ApLogic.HandleAliasRedirect)
            .WithDescription("Redirects to the URL associated with the alias.");

        group.MapGet("/aliases/{alias}", ApLogic.GetAlias)
            .WithOpenApi()
            .Produces<AliasEntryDto>()
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/aliases", ApLogic.PostAlias)
            .WithOpenApi()
            .Produces<AliasEntryDto>()
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}

using UrlAlias.Backend.DTos;
using UrlAlias.Backend.Dtos.Responses;

namespace UrlAlias.Backend.endpoints;

public static class EndpointMapper
{
    public static void MapAliasEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api");

        app.MapGet("uri/{alias}", ApLogic.HandleAliasRedirect)
            .WithDescription("Redirects to the URL associated with the alias.");

        group.MapGet("/aliases/{alias}", ApLogic.GetAlias)
            .WithOpenApi()
            .Produces<AliasCreatedResponse>()
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/aliases", ApLogic.PostAlias)
            .WithOpenApi()
            .Produces<AliasCreatedResponse>()
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/aliases", ApLogic.GetAllAliases)
            .WithOpenApi()
            .Produces<GetAliasesResponse>()
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithDescription("Fetches aliases with optional pagination and filtering.");
    }
}

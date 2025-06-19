using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace UrlAlias;

public static class AliasEndpoints
{
    public static void MapAliasEndpoints(this WebApplication app)
    {
        app.MapGet("/api/aliases", (AliasService svc) => svc.GetAll());

        app.MapGet("/api/aliases/{alias}", (string alias, AliasService svc) =>
        {
            var url = svc.TryGet(alias);
            return url is not null ? Results.Ok(url) : Results.NotFound();
        });

        app.MapPost("/api/aliases", (AliasEntry input, AliasService svc) =>
        {
            var result = svc.Add(input);
            return result == AddResult.Added
                ? Results.Created($"/api/aliases/{input.Alias}", input)
                : Results.Conflict(new { message = "Alias already exists" });
        });
    }
}

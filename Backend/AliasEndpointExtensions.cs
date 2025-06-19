using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace UrlAlias;

public static class AliasEndpointExtensions
{
    public static void MapAliasEndpoints(this WebApplication app)
    {
    
        app.MapGet("/api/aliases/{alias}", (string alias, IAliasService svc) =>
        {
            var url = svc.TryGet(alias);
            return url is not null ? Results.Ok(url) : Results.NotFound();
        });

        app.MapPost("/api/aliases", (AliasEntryDto input, IAliasService svc) =>
        {
            // Validate URL
            if (!UrlValidator.IsValid(input.Url))
                return Results.BadRequest(new { message = "Invalid URL" });
            
            var result = svc.Add(input.ToDomain());
            return result == AddResult.Added
                ? Results.Created($"/api/aliases/{input.Alias}", input)
                : Results.Conflict(new { message = "Alias already exists" });
        });
    }
}

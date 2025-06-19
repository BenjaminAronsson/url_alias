using UrlAlias.Dtos;
using UrlAlias.Models;
using UrlAlias.Services;

namespace UrlAlias.Extensions;

public static class AliasEndpointExtensions
{
    public static void MapAliasEndpoints(this WebApplication app)
    {

        app.MapGet("/{alias}", (string alias, HttpContext context, IAliasService svc) =>
        {
            var fallback =  "https://" + context.Request.Host.ToString() + "/swagger/index.html";
            var aliases = svc.TryGet(alias);
            
            return aliases is not null
                ? Results.Redirect(aliases)
                : Results.Redirect(fallback);
        });

        app.MapGet("/api/aliases/{alias}", (string alias, IAliasService svc) =>
        {
            var url = svc.TryGet(alias);
            return url is not null ? Results.Ok(url) : Results.NotFound();
        })
        .WithOpenApi();

        app.MapPost("/api/aliases", (AliasEntryDto input, HttpContext context, IAliasService svc, IUrlShortener shortener) =>
        {
            // Validate URL
            if (!UrlValidator.IsValid(input.Url))
                return Results.BadRequest(new { message = "Invalid URL" });

            // Generate alias if not provided
            if (string.IsNullOrWhiteSpace(input.Alias))
                input.Alias = shortener.GenerateAlias(input.Url);
            
            var result = svc.Add(input.ToDomain());

            //get host
            var host = "https://" + context.Request.Host.ToString();

            return result == AddResult.Added
                ? Results.Created($"{host}/{input.Alias}", input)
                : Results.Conflict(new { message = "Alias already exists" });
        })
        .WithOpenApi();
    }
}

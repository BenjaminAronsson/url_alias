using Microsoft.AspNetCore.Http.Extensions;
using Ulr_Alias.Backend.Services;
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
            var fallback = UriHelper.BuildAbsolute(
                context.Request.Scheme,
                context.Request.Host,
                context.Request.PathBase,
                "/swagger/index.html");
            var fallbackReturn = Results.Redirect(fallback, permanent: false, preserveMethod: true);
            var url = svc.TryGet(alias);
            
            if (string.IsNullOrWhiteSpace(url)) return fallbackReturn;
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return fallbackReturn;

            if (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp) return fallbackReturn;
            
            // (Optional) Forward the caller’s query string if the alias target doesn’t already have one
            var incomingQs = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty;
            if (string.IsNullOrEmpty(incomingQs))
                return Results.Redirect(uri.ToString(), permanent: true, preserveMethod: true);
            
            var builder = new UriBuilder(uri);
            if (!string.IsNullOrEmpty(builder.Query)) //this could be optional
                builder.Query = builder.Query.TrimStart('?') + "&" + incomingQs.TrimStart('?');
            
            uri = builder.Uri;
            return Results.Redirect(uri.ToString(), permanent: true, preserveMethod: true);
        });

        app.MapGet("/api/aliases/{alias}", (string alias, IAliasService svc) =>
        {
            var url = svc.TryGet(alias);
            
            return url is not null ? Results.Ok(url) : Results.NotFound();
        })
        .WithOpenApi();

        app.MapPost("/api/aliases", (AliasEntryRequest input, HttpContext context, IAliasService svc, IUrlShortener shortener) =>
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

using Microsoft.AspNetCore.Http.Extensions;
using UlrAlias.Backend.Dtos;
using UlrAlias.Backend.Models;
using UlrAlias.Backend.Services;
using UlrAlias.Backend.Validators;

namespace UlrAlias.Backend.endpoints;

public static class ApLogic
{
    public static async Task<IResult> HandleAliasRedirect(string alias, HttpContext context, IAliasService svc)
    {
        var fallback = UriHelper.BuildAbsolute(context.Request.Scheme, context.Request.Host, context.Request.PathBase,
            "/swagger/index.html");
        var fallbackReturn = Results.Redirect(fallback, false, true);
        var entry = await svc.TryGetAsync(alias);

        if (string.IsNullOrWhiteSpace(entry?.Url)) return fallbackReturn;
        if (!Uri.TryCreate(entry.Url, UriKind.Absolute, out var uri)) return fallbackReturn;

        if (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp) return fallbackReturn;

        //Optional, Forward the query string if the alias target doesn’t already have one
        var incomingQs = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty;
        if (uri.Query.Length != 0 || string.IsNullOrEmpty(incomingQs))
            return Results.Redirect(uri.ToString(), true, true);

        var builder = new UriBuilder(uri);
        if (!string.IsNullOrEmpty(builder.Query)) builder.Query = incomingQs;

        uri = builder.Uri;
        return Results.Redirect(uri.ToString(), true, true);
    }

    public static async Task<IResult> GetAlias(string alias, HttpContext context, IAliasService svc)
    {
        var url = await svc.TryGetAsync(alias);
        return url is not null ? Results.Ok(url) : Results.NotFound();
    }

    public static async Task<IResult> PostAlias(AliasEntryDto input, HttpContext context, IUrlShortener shortener,
        IAliasService svc)
    {
        // Validate URL
        if (!UrlValidator.IsValid(input.Url))
            return Results.BadRequest(new { message = "Invalid URL" });

        // Generate alias if not provided
        if (string.IsNullOrWhiteSpace(input.Alias))
            input.Alias = shortener.GenerateAlias(input.Url);

        var result = await svc.AddAsync(input.ToDomain());

        var uri = UriHelper.BuildAbsolute(
            context.Request.Scheme,
            context.Request.Host,
            string.Empty,
            EnsureLeadingSlash(input.Alias));

        return result == AddResult.Added
            ? Results.Created(uri, input)
            : Results.Conflict(new { message = "Alias already exists" });
    }

    private static string EnsureLeadingSlash(string? value)
    {
        return "/" + (value ?? string.Empty).TrimStart('/');
    }
}
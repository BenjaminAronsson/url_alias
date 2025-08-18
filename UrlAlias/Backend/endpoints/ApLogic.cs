using System.Globalization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using UrlAlias.Backend.DTos;
using UrlAlias.Backend.Dtos.Responses;
using UrlAlias.Backend.Extensions;
using UrlAlias.Backend.Models;
using UrlAlias.Backend.Services;
using UrlAlias.Backend.Validators;

namespace UrlAlias.Backend.endpoints;

public static class ApLogic
{
    public static async Task<IResult> HandleAliasRedirect(string alias, HttpContext context, IAliasService svc, CancellationToken cancellationToken)
    {
        var fallback = UriHelper.BuildAbsolute(context.Request.Scheme, context.Request.Host, context.Request.PathBase,
            "/swagger/index.html");
        var fallbackReturn = Results.Redirect(fallback, false, true);
        var entry = await svc.TryGetAsync(alias, cancellationToken);

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

    public static async Task<IResult> GetAlias(string alias, HttpContext context, IAliasService svc, CancellationToken cancellationToken)
    {
        var url = await svc.TryGetAsync(alias, cancellationToken);
        return url is not null ? Results.Ok(url) : Results.NotFound();
    }

    public static async Task<IResult> PostAlias(AliasEntryDto input, HttpContext context, IUrlShortener shortener,
        IAliasService svc, CancellationToken cancellationToken)
    {
        // Validate URL
        if (!UrlValidator.IsValid(input.Url))
            return Results.BadRequest(new { message = "Invalid URL" });

        // Generate alias if not provided
        if (string.IsNullOrWhiteSpace(input.Alias))
            input.Alias = shortener.GenerateAlias(input.Url);

        var result = await svc.AddAsync(input.ToDomain(), cancellationToken);

        var uri = UriHelper.BuildAbsolute(
            context.Request.Scheme,
            context.Request.Host,
            "uri".EnsureLeadingSlash(),
            input.Alias.EnsureLeadingSlash());

        var response = new AliasCreatedResponse(input, uri)
        {
            Url = input.Url
        };
        
        return result == AddResult.Added
            ? Results.Created(uri, response)
            : Results.Conflict(new { message = "Alias already exists" });
    }


    public static async Task<IResult> GetAllAliases([FromQuery(Name = "page")] int page, [FromQuery(Name = "pageSize")] int pageSize,
        IAliasService svc, HttpContext context, CancellationToken cancellationToken)
    {
        if(page < 1 || pageSize < 0 || pageSize > 100) Results.BadRequest("Invalid page");
        
        var numberOfAliasesTask = svc.CountAsync( cancellationToken);
        var aliasesTask = svc.FindAsync(page - 1, pageSize, cancellationToken);
        
        Task.WaitAll(aliasesTask, numberOfAliasesTask);
        
        var numberOfAliases = await numberOfAliasesTask;
        var aliases = await aliasesTask;
        
        var response = new GetAliasesResponse
        {
            Aliases = aliases.Select(a => new AliasCreatedResponse(
                    new AliasEntryDto
                    {
                        Alias = a.Alias,
                        Url = a.Url,
                        ExpiresAt = a.ExpiresAt
                    },
                    UriHelper.BuildAbsolute(
                        context.Request.Scheme,
                        context.Request.Host,
                        "uri".EnsureLeadingSlash(),
                        a.Alias.EnsureLeadingSlash())
                )
                {
                    Url = UriHelper.BuildAbsolute(
                        context.Request.Scheme,
                        context.Request.Host,
                        "uri".EnsureLeadingSlash(),
                        a.Alias.EnsureLeadingSlash())
                })
                .ToList(),
            TotalAliases = numberOfAliases,
            TotalPages = (int)Math.Ceiling((double)numberOfAliases / pageSize),
        };
        
        return Results.Ok(response);
    }
}
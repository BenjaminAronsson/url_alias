using System.Globalization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using UlrAlias.Backend.DTos;
using UlrAlias.Backend.Dtos.Responses;
using UlrAlias.Backend.Extensions;
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


    public static Task<IResult> GetAllAliases([FromQuery(Name = "page")] int page, [FromQuery(Name = "pageSize")] int pageSize, HttpContext context, IAliasService svc)
    {
        if(page < 1 || pageSize < 0 || pageSize > 100) Task.FromResult(Results.BadRequest("Invalid page"));
        const int numberOfAliases = 110;
        
        var response = new GetAliasesResponse
        {
            Aliases = [],
            TotalAliases = numberOfAliases,
            TotalPages = numberOfAliases % pageSize
        };
        
        var startIndex = (page - 1) * pageSize;
        
        for (var i = 0; i < numberOfAliases; i++)
        {
            if(startIndex > i) continue;
            if(response.Aliases.Count >= pageSize) break;
            
           var dummyAlias = new AliasEntryDto
            {
                Alias = i.ToString(CultureInfo.InvariantCulture),
                Url = "https://google.com",
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(30)
            };
           
           var alias = new AliasCreatedResponse(dummyAlias, 
               UriHelper.BuildAbsolute(context.Request.Scheme, context.Request.Host, "uri".EnsureLeadingSlash(), dummyAlias.Alias.EnsureLeadingSlash()))
           {
               Url = dummyAlias.Url
           };
           
           response.Aliases.Add(alias);
        }
        
        return Task.FromResult(Results.Ok(response));
    }
}
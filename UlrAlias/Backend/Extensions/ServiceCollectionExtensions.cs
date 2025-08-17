using UlrAlias.Backend.Services;

namespace UlrAlias.Backend.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUrlAliasServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<IAliasService, AliasService>();
        services.AddScoped<IUrlShortener, UrlShortener>();
        return services;
    }
}
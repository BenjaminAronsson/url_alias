using Ulr_Alias.Backend.Services;
using UrlAlias.Services;

namespace UrlAlias.Extensions;

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

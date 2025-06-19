using Ulr_Alias.Services;

namespace Ulr_Alias.Extensions;

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

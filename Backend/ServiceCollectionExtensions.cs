using Microsoft.Extensions.DependencyInjection;

namespace UrlAlias;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUrlAliasServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<AliasService>();
        return services;
    }
}

using Microsoft.Extensions.DependencyInjection;
using UrlAlias.Services;

namespace UrlAlias.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUrlAliasServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<IAliasService, AliasService>();
        return services;
    }
}

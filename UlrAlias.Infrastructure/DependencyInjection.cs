using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UlrAlias.Application.Services;
using UlrAlias.Infrastructure.Data;
using UlrAlias.Infrastructure.Services;

namespace UlrAlias.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.AddScoped<IAliasService, AliasService>();
        services.AddScoped<IUrlShortener, UrlShortener>();

        services.AddDbContext<AliasDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}

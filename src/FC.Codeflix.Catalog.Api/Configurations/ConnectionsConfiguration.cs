using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;


namespace FC.Codeflix.Catalog.Api.Configurations;

public static class ConnectionsConfiguration
{
    public static IServiceCollection AddAppConnections(this IServiceCollection services)
    {
        services.AddDbConnecion();
        return services;
    }
    public static IServiceCollection AddDbConnecion(this IServiceCollection services)
    {
        services.AddDbContext<CodeflixCatalogDbContext>(options => {
            options.UseInMemoryDatabase("InMemory-DSV-database");
        });
        return services;
    }
}

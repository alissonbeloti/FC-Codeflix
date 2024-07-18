using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;


namespace FC.Codeflix.Catalog.Api.Configurations;

public static class ConnectionsConfiguration
{
    public static IServiceCollection AddAppConnections(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbConnection(configuration);
        return services;
    }
    public static IServiceCollection AddDbConnection(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CatalogDb");
        services.AddDbContext<CodeflixCatalogDbContext>(options => {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });
        return services;
    }
}

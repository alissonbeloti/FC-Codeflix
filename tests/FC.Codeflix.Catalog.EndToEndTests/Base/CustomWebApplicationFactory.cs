using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;
public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup>
    where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("EndToEndTest");
        builder.ConfigureServices(services =>
        {
            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<CodeflixCatalogDbContext>();
                ArgumentNullException.ThrowIfNull(context, nameof(context));
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        });
        base.ConfigureWebHost(builder);
    }
}

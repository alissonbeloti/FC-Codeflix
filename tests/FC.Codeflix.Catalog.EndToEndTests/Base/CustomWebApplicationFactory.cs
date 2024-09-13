using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
using Google.Cloud.Storage.V1;
using Moq;
using FC.CodeFlix.Catalog.Infra.Message.Configuration;
using RabbitMQ.Client;
using Microsoft.Extensions.Options;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;
public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup>
    where TStartup : class
{
    public Mock<StorageClient> StorageClient { get; private set; }
    public IModel RabbitMQChannel { get; private set; }
    public RabbitMQConfiguration RabbitMQConfiguration { get; private set; }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("EndToEndTest");
        builder.ConfigureServices(services =>
        {
            var descriptor = services.First(s => s.ServiceType == typeof(StorageClient));
            services.Remove(descriptor);
            services.AddScoped(sp =>
            {
                StorageClient = new Mock<StorageClient>();
                return StorageClient.Object;
            });
            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                RabbitMQChannel = scope.ServiceProvider
                    .GetService<ChannelManager>()!
                    .GetChannel();
                RabbitMQConfiguration = scope.ServiceProvider
                    .GetService<IOptions<RabbitMQConfiguration>>()!
                    .Value;
                var context = scope.ServiceProvider.GetService<CodeflixCatalogDbContext>();
                ArgumentNullException.ThrowIfNull(context, nameof(context));
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        });
        base.ConfigureWebHost(builder);
    }
}

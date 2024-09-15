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
    : WebApplicationFactory<TStartup>, IDisposable
    where TStartup : class
{
    public string VideoCreatedQueue => "video.created.queue";
    public string VideoCreatedRoutingKey => "video.created";
    public string VideoEncodedRoutingKey => "video.encoded";
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
                SetupRabbitMQ();
                var context = scope.ServiceProvider.GetService<CodeflixCatalogDbContext>();
                ArgumentNullException.ThrowIfNull(context, nameof(context));
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        });
        base.ConfigureWebHost(builder);
    }
    public void SetupRabbitMQ()
    {
        var channel = RabbitMQChannel;
        var exchange = RabbitMQConfiguration!.Exchange;
        channel.ExchangeDeclare(exchange, "direct", true, true, null);
        channel.QueueDeclare(VideoCreatedQueue, true, false, false, null);
        channel.QueueBind(VideoCreatedQueue, exchange, VideoCreatedRoutingKey, null);

        channel.QueueDeclare(RabbitMQConfiguration.VideoEncodedQueue, true, false, false, null);
        channel.QueueBind(RabbitMQConfiguration.VideoEncodedQueue, exchange, 
            VideoEncodedRoutingKey, null);
    }

    public override ValueTask DisposeAsync()
    {
        TearDownRabbiMQ();
        return base.DisposeAsync();
    }

    private void TearDownRabbiMQ()
    {
        var channel = RabbitMQChannel;
        var exchange = RabbitMQConfiguration!.Exchange;
        channel.QueueUnbind(VideoCreatedQueue, exchange, VideoCreatedRoutingKey, null);
        channel.QueueDelete(VideoCreatedQueue, false, false);
        channel.QueueUnbind(RabbitMQConfiguration.VideoEncodedQueue, exchange, VideoEncodedRoutingKey, null);
        channel.QueueDelete(RabbitMQConfiguration.VideoEncodedQueue, false, false);
        channel.ExchangeDelete(exchange, false);
    }
}

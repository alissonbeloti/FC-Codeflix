using System.Text;
using RabbitMQ.Client;
using System.Text.Json;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Extensions;
using FC.Codeflix.Catalog.Api.ApiModels.Video;
using FC.CodeFlix.Catalog.Infra.Message.JsonPolicies;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;

[CollectionDefinition(nameof(VideoBaseFixture))]
public class VideoBaseFixtureCollection : ICollectionFixture<VideoBaseFixture> { }

public class VideoBaseFixture : GenreBaseFixture
{
    public VideoPersistence VideoPersistence { get; private set; }
    public CastMemberPersistence CastMemberPersistence { get; private set; }
    
    public VideoBaseFixture() : base()
    {
        VideoPersistence = new VideoPersistence(DbContext);
        CastMemberPersistence = new CastMemberPersistence(DbContext);
    }

    public void TearDownRabbitMQ()
    {
        var channel = WebAppFactory.RabbitMQChannel;
        var exchange = WebAppFactory.RabbitMQConfiguration!.Exchange;
        channel.QueueUnbind(WebAppFactory.VideoCreatedQueue, exchange, WebAppFactory.VideoCreatedRoutingKey, null);
        channel.QueueDelete(WebAppFactory.VideoCreatedQueue, false, false);
        channel.ExchangeDelete(exchange, false);
    }

    public (T?, uint) ReadMessageFromRabbitMQ<T>()
        where T: class
    {
        var consumingResult = WebAppFactory.RabbitMQChannel
            .BasicGet(WebAppFactory.VideoCreatedQueue, true);
        if (consumingResult is null) return (null, 0);
        var rawMessage = consumingResult.Body.ToArray();
        var stringMessage = Encoding.UTF8.GetString(rawMessage);
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new JsonSnakeCasePolicy()
        };
        var @event = JsonSerializer.Deserialize<T>(
            stringMessage, jsonOptions);
        return (@event, consumingResult.MessageCount);
    }

    public void PublishMessageToRabbitMQ(object exampleEvent)
    {
        var exchange = WebAppFactory.RabbitMQConfiguration.Exchange;
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new JsonSnakeCasePolicy()
        };
        var message = JsonSerializer.SerializeToUtf8Bytes(exampleEvent, jsonOptions);
        WebAppFactory.RabbitMQChannel.BasicPublish(exchange: exchange,
            routingKey: WebAppFactory.VideoEncodedRoutingKey, 
            body: message);

    }

    public void PurgeRabbitMQQueues()
    {
        IModel channel = WebAppFactory.RabbitMQChannel;
        channel.QueuePurge(WebAppFactory.VideoCreatedQueue);
        channel.QueuePurge(WebAppFactory.RabbitMQConfiguration.VideoEncodedQueue);
    }

    public CreateVideoApiInput GetBasicCreateVideoInput()
    {
        return new CreateVideoApiInput
        {
            Title = GetValidTitle(),
            Description = GetValidDescription(),
            Duration = GetValidDuration(),
            Opened = GetRandomBoolean(),
            Published = GetRandomBoolean(),
            Rating = GetRandomRationg().ToStringSignal(),
            YearLaunched = GetValidYearLaunched(),
        };
    }

    public string GetValidDescription()
        => Faker.Commerce.ProductDescription();

    public int GetValidDuration()
        => (new Random()).Next(80, 300);


    public string GetValidTitle()
        => (Faker.Commerce.Product() + Faker.Lorem.Letter(100)).Substring(0, 100);
    public string GetInvalidTooLongTitle()
        => (Faker.Commerce.Product() + Faker.Lorem.Letter(300));

    public string GetValidImagePath()
        => Faker.Image.PlaceImgUrl();

    public string GetValidMediaPath()
    {
        var exampleMedias = new String[]
        {
            "https://www.googlestorage.com/file-example.mp4",
            "https://www.storage.com/file-example.mp4",
            "https://www.s3.com.br/file-example.mp4",
            "https://www.glg.io/file-example.mp4",
        };
        var random = new Random();
        return exampleMedias[random.Next(exampleMedias.Length)];
    }

    public DomainEntity.Video GetValidVideo()
        => new DomainEntity.Video(
            GetValidTitle(),
            GetValidDescription(),
            GetRandomBoolean(),
            GetRandomBoolean(),
            GetValidYearLaunched(),
            GetValidDuration(),
            GetRandomRationg()
        );

    public DomainEntity.Video GetValidVideoWithAllProperties(string? title = null)
    {
        var video = new DomainEntity.Video(
                title ?? GetValidTitle(),
                GetValidDescription(),
                GetRandomBoolean(),
                GetRandomBoolean(),
                GetValidYearLaunched(),
                GetValidDuration(),
                GetRandomRationg()
            );

        video.UpdateBanner(GetValidImagePath());
        video.UpdateThumb(GetValidImagePath());
        video.UpdateThumbHalf(GetValidImagePath());
        video.UpdateMedia(GetValidMediaPath());
        video.UpdateTrailer(GetValidMediaPath());

        return video;
    }

    public Rating GetRandomRationg()
    {
        var values = Enum.GetValues<Rating>();
        var random = new Random();
        return values[random.Next(values.Length)];
    }

    public int GetValidYearLaunched()
        => Faker.Date.BetweenDateOnly(new DateOnly(1960, 1, 1), new DateOnly(2024, 1, 1)).Year;

    internal string GetTooLongTitle()
        => Faker.Lorem.Letter(400);

    internal string GetTooLongDescription()
        => Faker.Lorem.Letter(4001);

    internal DomainEntity.Media GetValidMedia() =>
        new(GetValidMediaPath());

    internal FileInput GetValidImageFileInput()
    {
        var exampleStream = new MemoryStream(Encoding.ASCII.GetBytes("teste"));
        var fileInput = new FileInput("jpg", exampleStream, "video/mp4");
        return fileInput;
    }

    internal FileInput GetValidMediaFileInput()
    {
        var exampleStream = new MemoryStream(Encoding.ASCII.GetBytes("teste"));
        var fileInput = new FileInput("mp4", exampleStream, "image/jpeg");
        return fileInput;
    }

    internal List<DomainEntity.Video> GetVideoCollection(int count = 10, bool withMedias = false)
    => Enumerable.Range(1, count).Select(_ => {
        var video = withMedias? GetValidVideoWithAllProperties(): GetValidVideo();
        return video;
        })
        .ToList();

    internal List<DomainEntity.Video> GetVideoCollection(IEnumerable<string> titles)
    => titles.Select(t => {
        var video = GetValidVideoWithAllProperties(t);

        return video;
    })
        .ToList();

    internal List<DomainEntity.Video> CloneVideoListOrdered(List<DomainEntity.Video> videos, string orderBy, SearchOrder dir)
    {
        var listClone = new List<DomainEntity.Video>(videos);
        var orderedEnumerable = (orderBy.ToLower(), dir) switch
        {
            ("title", SearchOrder.Asc) => listClone.OrderBy(x => x.Title).ThenBy(x => x.Id),
            ("title", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Title)
                .ThenByDescending(x => x.Id),
            ("id", SearchOrder.Asc) => listClone.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
            _ => listClone.OrderBy(x => x.Title)
                .ThenBy(x => x.Id),
        };
        return orderedEnumerable.ToList();
    }

}

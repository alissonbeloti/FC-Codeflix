using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Events;
using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
using FC.CodeFlix.Catalog.Infra.Message.DTOs;

using FluentAssertions;

using System.Runtime.CompilerServices;

namespace FC.Codeflix.Catalog.EndToEndTests.Worker;

[Collection(nameof(VideoBaseFixture))]
public class VideoEncodedEventConsumerTest: IDisposable
{
    private readonly VideoBaseFixture _fixture;

    public VideoEncodedEventConsumerTest(VideoBaseFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = nameof(EncodingSuccessesEventReceived))]
    [Trait("End2End/Worker", "VideoEncodedEventHandler - Event Handler")]
    private async Task EncodingSuccessesEventReceived()
    {
        var exampleVideos = _fixture.GetVideoCollection(5, true);
        await _fixture.VideoPersistence.InsertList(exampleVideos);
        var video = exampleVideos[2];
        var encodedFilePath = _fixture.GetValidMediaPath();
        var exampleEvent = new VideoEncodedMessageDTO
        {
            Video = new VideoEncodedMetadataDTO
            {
                EncodedVideoFolder = encodedFilePath,
                FilePath = video.Media!.FilePath,
                ResourceId = video.Id.ToString(),
            }
        };

        _fixture.PublishMessageToRabbitMQ(exampleEvent);

        await Task.Delay(800);
        var videoFromDB = await _fixture.VideoPersistence.GetById(video.Id);
        videoFromDB.Should().NotBeNull();
        videoFromDB!.Media!.Status.Should().Be(Domain.Enum.MediaStatus.Completed);
        videoFromDB.Media.EncondedPath.Should().Be(exampleEvent.Video.FullEncodedVideoFilePath);
        var (@event, count) = _fixture.ReadMessageFromRabbitMQ<object>();
        @event.Should().BeNull();
        count.Should().Be(0);
    }

    [Fact(DisplayName = nameof(EncodingFailedEventReceived))]
    [Trait("End2End/Worker", "VideoEncodedEventHandler - Event Handler")]
    private async Task EncodingFailedEventReceived()
    {
        var exampleVideos = _fixture.GetVideoCollection(5, true);
        await _fixture.VideoPersistence.InsertList(exampleVideos);
        var video = exampleVideos[2];
        var encodedFilePath = _fixture.GetValidMediaPath();
        var exampleEvent = new VideoEncodedMessageDTO
        {
            Message = new VideoEncodedMetadataDTO
            {
                FilePath = video.Media!.FilePath,
                ResourceId = video.Id.ToString()
            },
            Error = "There was an error on processing the video."
        };

        _fixture.PublishMessageToRabbitMQ(exampleEvent);

        await Task.Delay(800);
        var videoFromDB = await _fixture.VideoPersistence.GetById(video.Id);
        videoFromDB.Should().NotBeNull();
        videoFromDB!.Media!.Status.Should().Be(Domain.Enum.MediaStatus.Error);
        videoFromDB.Media.EncondedPath.Should().BeNull();
        var (@event, count) = _fixture.ReadMessageFromRabbitMQ<object>();
        @event.Should().BeNull();
        count.Should().Be(0);
    }

    [Fact(DisplayName = nameof(InvalidMessageEventReceived))]
    [Trait("End2End/Worker", "VideoEncodedEventHandler - Event Handler")]
    private async Task InvalidMessageEventReceived()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);
        var encodedFilePath = _fixture.GetValidMediaPath();
        var exampleEvent = new VideoEncodedMessageDTO
        {
            Message = new VideoEncodedMetadataDTO
            {
                FilePath = _fixture.GetValidMediaPath(),
                ResourceId = Guid.NewGuid().ToString(),
            },
            Error = "There was an error on processing the video."
        };

        _fixture.PublishMessageToRabbitMQ(exampleEvent);

        await Task.Delay(800);

        var (@event, count) = _fixture.ReadMessageFromRabbitMQ<object>();
        @event.Should().BeNull();
        count.Should().Be(0);
    }

    public void Dispose()
    {
        _fixture.CleanPersistence();
        _fixture.PurgeRabbitMQQueues();
    }
}

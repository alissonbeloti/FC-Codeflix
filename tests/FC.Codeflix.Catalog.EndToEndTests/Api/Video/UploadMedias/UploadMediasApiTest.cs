using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.Common.Stream;
using FC.Codeflix.Catalog.Domain.Events;
using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;

using FluentAssertions;

using Google.Apis.Upload;
using Google.Cloud.Storage.V1;

using Microsoft.AspNetCore.Mvc;

using Moq;

using System.Net;

using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.UploadMedias;

[Collection(nameof(VideoBaseFixture))]
public class UploadMediasApiTest : IDisposable
{
    private readonly VideoBaseFixture _fixture;

    public UploadMediasApiTest(VideoBaseFixture videoBaseFixture) => 
        _fixture = videoBaseFixture;

    [Fact(DisplayName = nameof(UploadBanner))]
    [Trait("EndToEnd/API", "Video/UpdateMedias - Endpoints")]
    public async Task UploadBanner()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);
        var videoId = exampleVideos[2].Id;
        var mediaType = "banner";
        var file = _fixture.GetValidImageFileInput();
        var expectedFileName = StorageName.Create(videoId, nameof(DomainEntity.Video.Banner), file.Extension);
        var expectedContent = file.FileStream.ToContentString();

        var (response, output) = await _fixture.ApiClient.PostFormData<object>(
            $"/videos/{videoId}/medias/{mediaType}", file);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDb = await _fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Banner!.Path.Should().Be(expectedFileName);
        _fixture.WebAppFactory.StorageClient!.Verify(
            x => x.UploadObjectAsync(It.IsAny<string>(),
            expectedFileName,
            file.ContentType,
            It.Is<Stream>(stream => stream.ToContentString() == expectedContent),
            It.IsAny<UploadObjectOptions>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<IProgress<IUploadProgress>>()
            ), Times.Once);
    }

    [Fact(DisplayName = nameof(UploadThumb))]
    [Trait("EndToEnd/API", "Video/UpdateMedias - Endpoints")]
    public async Task UploadThumb()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);
        var videoId = exampleVideos[2].Id;
        var mediaType = "thumbnail";
        var file = _fixture.GetValidImageFileInput();
        var expectedFileName = StorageName.Create(videoId, nameof(DomainEntity.Video.Thumb), file.Extension);
        var expectedContent = file.FileStream.ToContentString();

        var (response, output) = await _fixture.ApiClient.PostFormData<object>(
            $"/videos/{videoId}/medias/{mediaType}", file);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDb = await _fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Thumb!.Path.Should().Be(expectedFileName);
        _fixture.WebAppFactory.StorageClient!.Verify(
            x => x.UploadObjectAsync(It.IsAny<string>(),
            expectedFileName,
            file.ContentType,
            It.Is<Stream>(stream => stream.ToContentString() == expectedContent),
            It.IsAny<UploadObjectOptions>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<IProgress<IUploadProgress>>()
            ), Times.Once);
    }

    [Fact(DisplayName = nameof(UploadThumbHalf))]
    [Trait("EndToEnd/API", "Video/UpdateMedias - Endpoints")]
    public async Task UploadThumbHalf()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);
        var videoId = exampleVideos[2].Id;
        var mediaType = "thumbnail_half";
        var file = _fixture.GetValidImageFileInput();
        var expectedFileName = StorageName.Create(videoId, nameof(DomainEntity.Video.ThumbHalf), file.Extension);
        var expectedContent = file.FileStream.ToContentString();

        var (response, output) = await _fixture.ApiClient.PostFormData<object>(
            $"/videos/{videoId}/medias/{mediaType}", file);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDb = await _fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.ThumbHalf!.Path.Should().Be(expectedFileName);
        _fixture.WebAppFactory.StorageClient!.Verify(
            x => x.UploadObjectAsync(It.IsAny<string>(),
            expectedFileName,
            file.ContentType,
            It.Is<Stream>(stream => stream.ToContentString() == expectedContent),
            It.IsAny<UploadObjectOptions>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<IProgress<IUploadProgress>>()
            ), Times.Once);
    }

    [Fact(DisplayName = nameof(UploadTrailer))]
    [Trait("EndToEnd/API", "Video/UpdateMedias - Endpoints")]
    public async Task UploadTrailer()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);
        var videoId = exampleVideos[2].Id;
        var mediaType = "trailer";
        var file = _fixture.GetValidMediaFileInput();
        var expectedFileName = StorageName.Create(videoId, nameof(DomainEntity.Video.Trailer), file.Extension);
        var expectedContent = file.FileStream.ToContentString();

        var (response, output) = await _fixture.ApiClient.PostFormData<object>(
            $"/videos/{videoId}/medias/{mediaType}", file);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDb = await _fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Trailer!.FilePath.Should().Be(expectedFileName);
        _fixture.WebAppFactory.StorageClient!.Verify(
            x => x.UploadObjectAsync(It.IsAny<string>(),
            expectedFileName,
            file.ContentType,
            It.Is<Stream>(stream => stream.ToContentString() == expectedContent),
            It.IsAny<UploadObjectOptions>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<IProgress<IUploadProgress>>()
            ), Times.Once);
    }

    [Fact(DisplayName = nameof(UploadVideo))]
    [Trait("EndToEnd/API", "Video/UpdateMedias - Endpoints")]
    public async Task UploadVideo()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = exampleVideos[2].Id;
        var mediaType = "video";
        var file = _fixture.GetValidMediaFileInput();
        var expectedFileName = StorageName.Create(videoId, nameof(DomainEntity.Video.Media), file.Extension);
        var expectedContent = file.FileStream.ToContentString();

        var (response, output) = await _fixture.ApiClient.PostFormData<object>(
            $"/videos/{videoId}/medias/{mediaType}", file);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDb = await _fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Media!.FilePath.Should().Be(expectedFileName);
        _fixture.WebAppFactory.StorageClient!.Verify(
            x => x.UploadObjectAsync(It.IsAny<string>(),
            expectedFileName,
            file.ContentType,
            It.Is<Stream>(stream => stream.ToContentString() == expectedContent),
            It.IsAny<UploadObjectOptions>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<IProgress<IUploadProgress>>()
            ), Times.Once);
        var (@event, remainingMessages) = _fixture
            .ReadMessageFromRabbitMQ<VideoUploadedEvent>();
        remainingMessages.Should().Be(0);
        @event.Should().NotBeNull();
        @event!.FilePath.Should().Be(expectedFileName);
        @event.ResourceId.Should().Be(videoId);
        @event.OccuredOn.Should().NotBe(default);
        _fixture.PurgeRabbitMQQueues();
    }

    [Fact(DisplayName = nameof(Error422WhenMediaTypeIsInvalid))]
    [Trait("EndToEnd/API", "Video/UpdateMedias - Endpoints")]
    public async Task Error422WhenMediaTypeIsInvalid()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = exampleVideos[2].Id;
        var mediaType = "thumb";
        var file = _fixture.GetValidImageFileInput();

        var (response, output) = await _fixture.ApiClient.PostFormData<ProblemDetails>(
            $"/videos/{videoId}/medias/{mediaType}", file);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("UnprocessableEntity");
        output.Detail.Should().Be($"'{mediaType}' is not a valid media type.");

    }

    [Fact(DisplayName = nameof(Error404WhenVideoIdIsNotFound))]
    [Trait("EndToEnd/API", "Video/UpdateMedias - Endpoints")]
    public async Task Error404WhenVideoIdIsNotFound()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = Guid.NewGuid();
        var mediaType = "banner";
        var file = _fixture.GetValidImageFileInput();

        var (response, output) = await _fixture.ApiClient.PostFormData<ProblemDetails>(
            $"/videos/{videoId}/medias/{mediaType}", file);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Video '{videoId}' not found.");

    }

    public void Dispose() => _fixture.CleanPersistence();
}

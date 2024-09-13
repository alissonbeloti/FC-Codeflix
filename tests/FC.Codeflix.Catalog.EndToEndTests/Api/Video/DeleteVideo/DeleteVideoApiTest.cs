using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;

using FluentAssertions;

using Google.Cloud.Storage.V1;

using Microsoft.AspNetCore.Mvc;

using Moq;

using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.DeleteVideo;

[Collection(nameof(VideoBaseFixture))]
public class DeleteVideoApiTest : IDisposable
{
    private readonly VideoBaseFixture _fixture;

    public DeleteVideoApiTest(VideoBaseFixture fixture)
    {
        _fixture = fixture;
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
                "H:\\estudos\\fullcycle\\FC.Codeflix.Catalog\\src\\FC.Codeflix.Catalog.Api\\gcp_credentials.json");
    }

    [Fact(DisplayName = nameof(DeleteVideo))]
    [Trait("EndToEnd/API", "Video/DeleteVideo - Endpoints")]
    public async Task DeleteVideo()
    {
        var examples = _fixture.GetVideoCollection(10);
        await _fixture.VideoPersistence.InsertList(examples);
        var videoId = examples[7].Id;

        var (response, output) = await _fixture.ApiClient
            .Delete<object>($"/videos/{videoId}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDB = await _fixture.VideoPersistence.GetById(videoId);
        videoFromDB.Should().BeNull();
    }

    [Fact(DisplayName = nameof(Error404WhenVideoIdNotFound))]
    [Trait("EndToEnd/API", "Video/DeleteVideo - Endpoints")]
    public async Task Error404WhenVideoIdNotFound()
    {
        var examples = _fixture.GetVideoCollection(2);
        await _fixture.VideoPersistence.InsertList(examples);
        var videoId = Guid.NewGuid();

        var (response, output) = await _fixture.ApiClient
            .Delete<ProblemDetails>($"/videos/{videoId}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Video '{videoId}' not found.");
    }

    [Fact(DisplayName = nameof(DeleteVideoWithRelationships))]
    [Trait("EndToEnd/API", "Video/DeleteVideo - Endpoints")]
    public async Task DeleteVideoWithRelationships()
    {
        var categories = _fixture.GetExampleCategoryList(3);
        var genres = _fixture.GetExampleGenresList(4);
        var castMembers = _fixture.GetExampleCastMemberList(5);
        var exampleVideos = _fixture.GetVideoCollection(10, true);
        exampleVideos.ForEach(video =>
        {
            categories.ForEach(category => video.AddCategory(category.Id));
            genres.ForEach(genre => video.AddGenre(genre.Id));
            castMembers.ForEach(castMember => video.AddCastMember(castMember.Id));
        });
        await _fixture.CategoryPersitence.InsertList(categories);
        await _fixture.GenrePersistence.InsertList(genres);
        await _fixture.CastMemberPersistence.InsertList(castMembers);
        await _fixture.VideoPersistence.InsertList(exampleVideos);
        var mediaCount = await _fixture.VideoPersistence.GetMediaCount();
        var expectedMediaCount = mediaCount - 2;
        var video = exampleVideos[7];
        var videoId = video.Id;
        var allMedias = new[] { 
            video.Trailer!.FilePath,
            video.Media!.FilePath,
            video.Banner!.Path,
            video.Thumb!.Path,
            video.ThumbHalf!.Path,
        };

        var (response, output) = await _fixture.ApiClient
            .Delete<object>($"/videos/{videoId}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output!.Should().BeNull();
        var videoFromDB = await _fixture.VideoPersistence.GetById(videoId);
        videoFromDB.Should().BeNull();
        var categoriesFromDb = await _fixture.VideoPersistence.GetVideosCategories(videoId);
        categoriesFromDb.Should().BeEmpty();
        var castMembersFromDb = await _fixture.VideoPersistence.GetVideosCastMembers(videoId);
        castMembersFromDb.Should().BeEmpty();
        var genresFromDb = await _fixture.VideoPersistence.GetVideosGenres(videoId);
        genresFromDb.Should().BeEmpty();
        var actualMediaCount = await _fixture.VideoPersistence.GetMediaCount();
        actualMediaCount.Should().Be(expectedMediaCount);
        _fixture.WebAppFactory.StorageClient!.Verify(x =>
            x.DeleteObjectAsync(It.IsAny<string>(), 
            It.Is<string>(fileName => allMedias.Contains(fileName)),
            It.IsAny<DeleteObjectOptions>(), 
            It.IsAny<CancellationToken>()), Times.Exactly(5));
        _fixture.WebAppFactory.StorageClient!.Verify(x =>
            x.DeleteObjectAsync(It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<DeleteObjectOptions>(),
            It.IsAny<CancellationToken>()), Times.Exactly(5));
    }

    public void Dispose() => _fixture.CleanPersistence();
}

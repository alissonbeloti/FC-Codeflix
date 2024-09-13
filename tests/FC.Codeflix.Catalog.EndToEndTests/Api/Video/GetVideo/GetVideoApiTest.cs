using System.Net;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Extensions;
using FC.Codeflix.Catalog.EndToEndTests.Models;
using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using Microsoft.AspNetCore.Mvc;



namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.GetVideo;
[Collection(nameof(VideoBaseFixture))]
public class GetVideoApiTest(VideoBaseFixture fixture) : IDisposable
{
    [Fact(DisplayName = nameof(GetVideo))]
    [Trait("EndToEnd/API", "Video/GetVideos - Endpoints")]
    private async Task GetVideo()
    {
        var categories = fixture.GetExampleCategoryList(3);
        var genres = fixture.GetExampleGenresList(4);
        var castMembers = fixture.GetExampleCastMemberList(5);
        var exampleVideos = fixture.GetVideoCollection(10);
        exampleVideos.ForEach(video =>
        {
            categories.ForEach(category => video.AddCategory(category.Id));
            genres.ForEach(genre => video.AddGenre(genre.Id));
            castMembers.ForEach(castMember => video.AddCastMember(castMember.Id));
        });
        await fixture.CategoryPersitence.InsertList(categories);
        await fixture.GenrePersistence.InsertList(genres);
        await fixture.CastMemberPersistence.InsertList(castMembers);
        await fixture.VideoPersistence.InsertList(exampleVideos);
        var exampleVideo = exampleVideos.ElementAt(5);

        var (response, output) = await fixture.ApiClient
            .Get<TestApiResponse<VideoModelOutput>>($"/videos/{exampleVideo.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Should().NotBeNull();
        exampleVideo!.Title.Should().Be(output.Data!.Title);
        exampleVideo.Description.Should().Be(output.Data.Description);
        exampleVideo.Duration.Should().Be(output.Data.Duration);
        exampleVideo.Opened.Should().Be(output.Data.Opened);
        exampleVideo.Published.Should().Be(output.Data.Published);
        exampleVideo.Rating.Should().Be(output.Data.Rating.ToRating());
            var expectedCategories = categories.Select(category =>
                new VideoModelOutputRelatedAggregate(category.Id,
                null));
            var expectedGenres = genres.Select(genre =>
                new VideoModelOutputRelatedAggregate(genre.Id,
                null));
            var expectedCastMembers = castMembers.Select(castMember =>
                new VideoModelOutputRelatedAggregate(castMember.Id, null));
        output.Data.Categories.Should().BeEquivalentTo(expectedCategories);
        output.Data.Genres.Should().BeEquivalentTo(expectedGenres);
        output.Data.CastMembers.Should().BeEquivalentTo(expectedCastMembers);
    }

    [Fact(DisplayName = nameof(Error404IdNotFound))]
    [Trait("EndToEnd/API", "Video/GetVideos - Endpoints")]
    private async Task Error404IdNotFound()
    {
        var exampleVideos = fixture.GetVideoCollection(10);
        await fixture.VideoPersistence.InsertList(exampleVideos);
        var exampleVideoId = Guid.NewGuid();

        var (response, output) = await fixture.ApiClient
            .Get<ProblemDetails>($"/videos/{exampleVideoId}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Video '{exampleVideoId}' not found.");
    }
    public void Dispose() => fixture.CleanPersistence();

}

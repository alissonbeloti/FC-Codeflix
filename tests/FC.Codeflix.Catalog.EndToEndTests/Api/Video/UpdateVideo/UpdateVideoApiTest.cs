using FC.Codeflix.Catalog.Api.ApiModels.Video;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Extensions;
using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
using FC.Codeflix.Catalog.EndToEndTests.Models;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.UpdateVideo;
[Collection(nameof(VideoBaseFixture))]
public class UpdateVideoApiTest : IDisposable
{
    private readonly VideoBaseFixture _fixture;

    public UpdateVideoApiTest(VideoBaseFixture fixture)
    {
        _fixture = fixture;
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
                "H:\\estudos\\fullcycle\\FC.Codeflix.Catalog\\src\\FC.Codeflix.Catalog.Api\\gcp_credentials.json");
    }

    [Fact(DisplayName = nameof(UpdateBasicVideo))]
    [Trait("EndToEnd/API", "Video/UpdateVideo - Endpoints")]
    public async Task UpdateBasicVideo()
    {

        var exampleVideos = _fixture.GetVideoCollection(10, false);
        await _fixture.VideoPersistence.InsertList(exampleVideos);
        var targetId = exampleVideos.ElementAt(5).Id;
        var input = new UpdateVideoApiInput
        {
            Title = _fixture.GetValidTitle(),
            Description = _fixture.GetValidDescription(),
            Duration = _fixture.GetValidDuration(),
            Opened = _fixture.GetRandomBoolean(),
            Published = _fixture.GetRandomBoolean(),
            Rating = _fixture.GetRandomRationg().ToStringSignal(),
            YearLaunched = _fixture.GetValidYearLaunched(),
        };

        var (response, output) = await _fixture.ApiClient
            .Put<TestApiResponse<VideoModelOutput>>($"/videos/{targetId}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Should().NotBeNull();
        input!.Title.Should().Be(output.Data!.Title);
        input.Description.Should().Be(output.Data.Description);
        input.Duration.Should().Be(output.Data.Duration);
        input.Opened.Should().Be(output.Data.Opened);
        input.Published.Should().Be(output.Data.Published);
        input.Rating.Should().Be(output.Data.Rating);
        var videoFromDb = await _fixture.VideoPersistence.GetById(targetId);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Id.Should().Be(targetId);
        videoFromDb.Title.Should().Be(input.Title);
        videoFromDb.Description.Should().Be(input.Description);
        videoFromDb.YearLaunched.Should().Be(input.YearLaunched);
        videoFromDb.Opened.Should().Be(input.Opened);
        videoFromDb.Published.Should().Be(input.Published);
        videoFromDb.Duration.Should().Be(input.Duration);
        videoFromDb.Rating.ToStringSignal().Should().Be(input.Rating);
    }

    [Fact(DisplayName = nameof(UpdateBasicVideoWithRelationships))]
    [Trait("EndToEnd/API", "Video/UpdateVideo - Endpoints")]
    public async Task UpdateBasicVideoWithRelationships()
    {
        var categories = _fixture.GetExampleCategoryList(3);
        var genres = _fixture.GetExampleGenresList(4);
        var castMembers = _fixture.GetExampleCastMemberList(5);
        var exampleVideos = _fixture.GetVideoCollection(10);
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
        var targetId = exampleVideos.ElementAt(5).Id;
        var targetCategories = new[]
        {
            categories.ElementAt(1)
        };
        var targetGenres = new[]
        {
            genres.ElementAt(0),
            genres.ElementAt(2),
        };
        var targetCastMembers = new[]
        {
            castMembers.ElementAt(1),
            castMembers.ElementAt(2),
            castMembers.ElementAt(3),
        };

        var input = new UpdateVideoApiInput
        {
            Title = _fixture.GetValidTitle(),
            Description = _fixture.GetValidDescription(),
            Duration = _fixture.GetValidDuration(),
            Opened = _fixture.GetRandomBoolean(),
            Published = _fixture.GetRandomBoolean(),
            Rating = _fixture.GetRandomRationg().ToStringSignal(),
            YearLaunched = _fixture.GetValidYearLaunched(),
            CategoriesIds = targetCategories.Select(x => x.Id).ToList(),
            GenresIds = targetGenres.Select(x => x.Id).ToList(),
            CastMembersIds = targetCastMembers.Select(x => x.Id).ToList(),
        };

        var (response, output) = await _fixture.ApiClient
            .Put<TestApiResponse<VideoModelOutput>>($"/videos/{targetId}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Should().NotBeNull();
        input!.Title.Should().Be(output.Data!.Title);
        input.Description.Should().Be(output.Data.Description);
        input.Duration.Should().Be(output.Data.Duration);
        input.Opened.Should().Be(output.Data.Opened);
        input.Published.Should().Be(output.Data.Published);
        input.Rating.Should().Be(output.Data.Rating);
        var expectedCategories = targetCategories.Select(category => 
            new VideoModelOutputRelatedAggregate(category.Id))
            .ToList();
        output.Data.Categories.Should().BeEquivalentTo(expectedCategories);
        var expectedGenres = targetGenres.Select(genre =>
            new VideoModelOutputRelatedAggregate(genre.Id))
            .ToList();
        output.Data.Genres.Should().BeEquivalentTo(expectedGenres);
        var expectedCastMembers = targetCastMembers.Select(castMember =>
            new VideoModelOutputRelatedAggregate(castMember.Id))
            .ToList();
        output.Data.CastMembers.Should().BeEquivalentTo(expectedCastMembers);
        var videoFromDb = await _fixture.VideoPersistence.GetById(targetId);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Id.Should().Be(targetId);
        videoFromDb.Title.Should().Be(input.Title);
        videoFromDb.Description.Should().Be(input.Description);
        videoFromDb.YearLaunched.Should().Be(input.YearLaunched);
        videoFromDb.Opened.Should().Be(input.Opened);
        videoFromDb.Published.Should().Be(input.Published);
        videoFromDb.Duration.Should().Be(input.Duration);
        videoFromDb.Rating.ToStringSignal().Should().Be(input.Rating);
        var categoriesFromDb = await _fixture.VideoPersistence.GetVideosCategories(targetId);
        var categoriesIdsFromDb = categoriesFromDb!.Select(x => x.CategoryId).ToList();
        input.CategoriesIds.Should().BeEquivalentTo(categoriesIdsFromDb);
        var genresFromDb = await _fixture.VideoPersistence.GetVideosGenres(targetId);
        var genresIdsFromDb = genresFromDb!.Select(x => x.GenreId).ToList();
        input.GenresIds.Should().BeEquivalentTo(genresIdsFromDb);
        var castMemberFromDb = await _fixture.VideoPersistence.GetVideosCastMembers(targetId);
        var castMemberIdsFromDb = castMemberFromDb!.Select(x => x.CastMemberId).ToList();
        input.CastMembersIds.Should().BeEquivalentTo(castMemberIdsFromDb);
    }

    [Fact(DisplayName = nameof(Error404WhenVideoIdNotFound))]
    [Trait("EndToEnd/API", "Video/UpdateVideo - Endpoints")]
    public async Task Error404WhenVideoIdNotFound()
    {
        var exampleVideos = _fixture.GetVideoCollection(10, false);
        await _fixture.VideoPersistence.InsertList(exampleVideos);
        var targetId = Guid.NewGuid();
        var input = new UpdateVideoApiInput
        {
            Title = _fixture.GetValidTitle(),
            Description = _fixture.GetValidDescription(),
            Duration = _fixture.GetValidDuration(),
            Opened = _fixture.GetRandomBoolean(),
            Published = _fixture.GetRandomBoolean(),
            Rating = _fixture.GetRandomRationg().ToStringSignal(),
            YearLaunched = _fixture.GetValidYearLaunched(),
        };

        var (response, output) = await _fixture.ApiClient
            .Put<ProblemDetails>($"/videos/{targetId}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Video '{targetId}' not found.");
    }

    [Fact(DisplayName = nameof(Error422WhenCategoryIdNotFound))]
    [Trait("EndToEnd/API", "Video/UpdateVideo - Endpoints")]
    public async Task Error422WhenCategoryIdNotFound()
    {
        var exampleVideos = _fixture.GetVideoCollection(10, false);
        await _fixture.VideoPersistence.InsertList(exampleVideos);
        var categoryId = Guid.NewGuid();
        var targetId = exampleVideos.ElementAt(4).Id;
        var input = new UpdateVideoApiInput
        {
            Title = _fixture.GetValidTitle(),
            Description = _fixture.GetValidDescription(),
            Duration = _fixture.GetValidDuration(),
            Opened = _fixture.GetRandomBoolean(),
            Published = _fixture.GetRandomBoolean(),
            Rating = _fixture.GetRandomRationg().ToStringSignal(),
            YearLaunched = _fixture.GetValidYearLaunched(),
            CategoriesIds = new List<Guid> { categoryId },
        };

        var (response, output) = await _fixture.ApiClient
            .Put<ProblemDetails>($"/videos/{targetId}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related category id (or ids) not found: {categoryId}.");
    }

    public void Dispose() => _fixture.CleanPersistence();
}

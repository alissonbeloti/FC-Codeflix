using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Api.ApiModels.Video;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Extensions;
using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.CreateVideo;

[Collection(nameof(VideoBaseFixture))]
public class CreateVideoApiTest(VideoBaseFixture fixture) : IDisposable
{
    [Fact(DisplayName = nameof(CreateBasicVideo))]
    [Trait("EndToEnd/API", "Video/CreateVideo - Endpoints")]
    public async Task CreateBasicVideo ()
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
                "H:\\estudos\\fullcycle\\FC.Codeflix.Catalog\\src\\FC.Codeflix.Catalog.Api\\gcp_credentials.json");
        CreateVideoApiInput input = fixture.GetBasicCreateVideoInput();

        var (response, output) = await fixture.ApiClient.
            Post<ApiResponse<VideoModelOutput>>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.Created);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Title.Should().Be(input.Title);
        output.Data.Description.Should().Be(input.Description);
        output.Data.YearLaunched.Should().Be(input.YearLaunched);
        output.Data.Opened.Should().Be(input.Opened);
        output.Data.Published.Should().Be(input.Published);
        output.Data.Duration.Should().Be(input.Duration);
        output.Data.Rating.Should().Be(input.Rating);
        var videoFromDb = await fixture.VideoPersistence.GetById(output.Data.Id);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Id.Should().NotBeEmpty();
        videoFromDb.Title.Should().Be(input.Title);
        videoFromDb.Description.Should().Be(input.Description);
        videoFromDb.YearLaunched.Should().Be(input.YearLaunched);
        videoFromDb.Opened.Should().Be(input.Opened);
        videoFromDb.Published.Should().Be(input.Published);
        videoFromDb.Duration.Should().Be(input.Duration);
        videoFromDb.Rating.ToStringSignal().Should().Be(input.Rating);

    }

    [Fact(DisplayName = nameof(CreateVideoWithRelationships))]
    [Trait("EndToEnd/API", "Video/CreateVideo - Endpoints")]
    public async Task CreateVideoWithRelationships()
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
                "H:\\estudos\\fullcycle\\FC.Codeflix.Catalog\\src\\FC.Codeflix.Catalog.Api\\gcp_credentials.json");
        var categories = fixture.GetExampleCategoryList();
        await fixture.CategoryPersitence.InsertList(categories);
        var genres = fixture.GetExampleGenresList();
        await fixture.GenrePersistence.InsertList(genres);
        var castMembers = fixture.GetExampleCastMemberList();
        await fixture.CastMemberPersistence.InsertList(castMembers);

        CreateVideoApiInput input = fixture.GetBasicCreateVideoInput();
        input.CategoriesIds = categories.Select(c => c.Id).ToList();
        input.GenresIds = genres.Select(c => c.Id).ToList();
        input.CastMembersIds = castMembers.Select(c => c.Id).ToList();

        var (response, output) = await fixture.ApiClient.
            Post<ApiResponse<VideoModelOutput>>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.Created);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Title.Should().Be(input.Title);
        output.Data.Description.Should().Be(input.Description);
        output.Data.YearLaunched.Should().Be(input.YearLaunched);
        output.Data.Opened.Should().Be(input.Opened);
        output.Data.Published.Should().Be(input.Published);
        output.Data.Duration.Should().Be(input.Duration);
        output.Data.Rating.Should().Be(input.Rating);
        var outputCategoryIds = output.Data.Categories!.Select(c => c.Id).ToList();
        outputCategoryIds.Should().BeEquivalentTo(input.CategoriesIds);
        var outputGenreIds = output.Data.Genres!.Select(c => c.Id).ToList();
        outputGenreIds.Should().BeEquivalentTo(input.GenresIds);
        var outputCastMemberIds = output.Data.CastMembers!.Select(c => c.Id).ToList();
        outputCastMemberIds.Should().BeEquivalentTo(input.CastMembersIds);
        var videoFromDb = await fixture.VideoPersistence.GetById(output.Data.Id);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Id.Should().NotBeEmpty();
        videoFromDb.Title.Should().Be(input.Title);
        videoFromDb.Description.Should().Be(input.Description);
        videoFromDb.YearLaunched.Should().Be(input.YearLaunched);
        videoFromDb.Opened.Should().Be(input.Opened);
        videoFromDb.Published.Should().Be(input.Published);
        videoFromDb.Duration.Should().Be(input.Duration);
        videoFromDb.Rating.ToStringSignal().Should().Be(input.Rating);
        var categoriesFromDb = await fixture.VideoPersistence.GetVideosCategories(output.Data.Id);
        categoriesFromDb.Should().NotBeNull();
        categoriesFromDb!.Select(x => x.CategoryId)
            .Should().BeEquivalentTo(input.CategoriesIds);
        var genresFromDb = await fixture.VideoPersistence.GetVideosGenres(output.Data.Id);
        genresFromDb.Should().NotBeNull();
        genresFromDb!.Select(x => x.GenreId).Should().BeEquivalentTo(input.GenresIds);
        var castMembersFromDb = await fixture.VideoPersistence.GetVideosCastMembers(output.Data.Id);
        castMembersFromDb.Should().NotBeNull();
        castMembersFromDb!.Select(x => x.CastMemberId).Should()
            .BeEquivalentTo(input.CastMembersIds);
    }

    [Fact(DisplayName = nameof(CreateVideoWithInvalidGenreId))]
    [Trait("EndToEnd/API", "Video/CreateVideo - Endpoints")]
    public async Task CreateVideoWithInvalidGenreId()
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
                "H:\\estudos\\fullcycle\\FC.Codeflix.Catalog\\src\\FC.Codeflix.Catalog.Api\\gcp_credentials.json");
        var invalidGenreId = Guid.NewGuid();
        CreateVideoApiInput input = fixture.GetBasicCreateVideoInput();
        input.GenresIds = new List<Guid> { invalidGenreId };

        var (response, output) = await fixture.ApiClient.
            Post<ProblemDetails>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("RelatedAggregate");
        output!.Detail.Should().Be($"Related genre id (or ids) not found: {invalidGenreId}.");
    }

    [Fact(DisplayName = nameof(CreateVideoWithInvalidCategoryId))]
    [Trait("EndToEnd/API", "Video/CreateVideo - Endpoints")]
    public async Task CreateVideoWithInvalidCategoryId()
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
                "H:\\estudos\\fullcycle\\FC.Codeflix.Catalog\\src\\FC.Codeflix.Catalog.Api\\gcp_credentials.json");
        var invalidCategoryId = Guid.NewGuid();
        CreateVideoApiInput input = fixture.GetBasicCreateVideoInput();
        input.CategoriesIds = new List<Guid> { invalidCategoryId };

        var (response, output) = await fixture.ApiClient.
            Post<ProblemDetails>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("RelatedAggregate");
        output!.Detail.Should().Be($"Related category id (or ids) not found: {invalidCategoryId}.");
    }

    [Fact(DisplayName = nameof(CreateVideoWithInvalidCastMemberId))]
    [Trait("EndToEnd/API", "Video/CreateVideo - Endpoints")]
    public async Task CreateVideoWithInvalidCastMemberId()
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
                "H:\\estudos\\fullcycle\\FC.Codeflix.Catalog\\src\\FC.Codeflix.Catalog.Api\\gcp_credentials.json");
        var invalidCastMemberId = Guid.NewGuid();
        CreateVideoApiInput input = fixture.GetBasicCreateVideoInput();
        input.CastMembersIds = new List<Guid> { invalidCastMemberId };

        var (response, output) = await fixture.ApiClient.
            Post<ProblemDetails>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("RelatedAggregate");
        output!.Detail.Should().Be($"Related castMember id (or ids) not found: {invalidCastMemberId}.");
    }

    public void Dispose() => fixture.CleanPersistence();
}

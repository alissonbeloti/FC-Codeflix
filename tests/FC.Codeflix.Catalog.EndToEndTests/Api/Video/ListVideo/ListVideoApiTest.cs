using FC.Codeflix.Catalog.Domain.Extensions;
using FC.Codeflix.Catalog.EndToEndTests.Models;
using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.ListVideo;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

using FluentAssertions;

using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.ListVideo;
[Collection(nameof(VideoBaseFixture))]
public class ListVideoApiTest(VideoBaseFixture fixture) : IDisposable
{
    [Fact(DisplayName = nameof(ListVideoBasic))]
    [Trait("EndToEnd/API", "Video/ListVideos - Endpoints")]
    private async Task ListVideoBasic()
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
        
        var input = new ListVideosInput(1, exampleVideos.Count);

        var (response, output) = await fixture.ApiClient
            .Get<TestApiResponseList<VideoModelOutput>>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Meta!.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data.Should().NotBeNull();
        output.Data!.Count.Should().Be(exampleVideos.Count);
        output.Data.ForEach(outputItem => { 
            outputItem.Should().NotBeNull(); 
            var exampleItem = exampleVideos.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            exampleItem!.Title.Should().Be(outputItem.Title);
            exampleItem.Description.Should().Be(outputItem.Description);
            exampleItem.Duration.Should().Be(outputItem.Duration);
            exampleItem.Opened.Should().Be(outputItem.Opened);
            exampleItem.Published.Should().Be(outputItem.Published);
            exampleItem.Rating.Should().Be(outputItem.Rating.ToRating());
            var expectedCategories = categories.Select(category => 
                new VideoModelOutputRelatedAggregate(category.Id,
                category.Name));
            var expectedGenres = genres.Select(genre => 
                new VideoModelOutputRelatedAggregate(genre.Id,
                genre.Name));
            var expectedCastMembers = castMembers.Select(castMember =>
                new VideoModelOutputRelatedAggregate(castMember.Id, castMember.Name));
            outputItem.Categories.Should().BeEquivalentTo(expectedCategories);
            outputItem.Genres.Should().BeEquivalentTo(expectedGenres);
            outputItem.CastMembers.Should().BeEquivalentTo(expectedCastMembers);
        });
    }

    [Fact(DisplayName = nameof(ReturnsEmptywhenThereIsNoVideo))]
    [Trait("EndToEnd/API", "Video/ListVideos - Endpoints")]
    private async Task ReturnsEmptywhenThereIsNoVideo()
    {
        var input = new ListVideosInput(1, 10);

        var (response, output) = await fixture.ApiClient
            .Get<TestApiResponseList<VideoModelOutput>>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Meta!.Total.Should().Be(0);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data.Should().NotBeNull();
        output.Data!.Should().BeEmpty();
        
    }

    [Theory(DisplayName = nameof(ListPaginated))]
    [Trait("EndToEnd/API", "Video/ListVideos - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListPaginated(int quantityToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems)
    {
        var exampleVideos = fixture.GetVideoCollection(quantityToGenerate, false);
        await fixture.VideoPersistence.InsertList(exampleVideos);
        var input = new ListVideosInput(page, perPage);

        var (response, output) = await fixture.ApiClient
            .Get<TestApiResponseList<VideoModelOutput>>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(quantityToGenerate);
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(expectedQuantityItems);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
    }

    [Theory(DisplayName = nameof(Ordered))]
    [Trait("EndToEnd/API", "Video/ListVideos - Endpoints")]
    [InlineData("title", "asc")]
    [InlineData("title", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "desc")]
    public async Task Ordered(string orderBy, string order)
    {
        var exampleVideoList = fixture.GetVideoCollection(10, false);
        await fixture.VideoPersistence.InsertList(exampleVideoList);

        var input = new ListVideosInput();
        input.Page = 1;
        input.PerPage = exampleVideoList.Count;
        input.Dir = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        input.Sort = orderBy;

        var (response, output) = await fixture.ApiClient
            .Get<TestApiResponseList<VideoModelOutput>>("videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(10);
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(10);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        var expectedOrderedList = fixture.CloneVideoListOrdered(
              exampleVideoList,
              orderBy,
              input.Dir);
        output.Data.Should().Equal(expectedOrderedList, (v1, v2) => v1.Id == v2.Id);
    }

    [Theory(DisplayName = nameof(SearchVideos))]
    [Trait("EndToEnd/API", "Video/ListVideos - Endpoints")]
    [InlineData("007", 1, 2, 2, 4)]
    [InlineData("st", 2, 2, 1, 3)]
    [InlineData("007: Casino", 1, 2, 1, 1)]
    [InlineData("Terminator", 1, 5, 0, 0)]
    public async Task SearchVideos(string searchTerm,
        int page,
        int perPage,
        int expectedReturnedtems,
        int expectedTotalItems)
    {
        var movieNames = new[]
        {
            "007: Dr. No",
            "007: Casino Royale",
            "007: GoldFinger",
            "007: Skyfall",
            "Star Wars: Return of the Jedi",
            "Star Wars: The Empire Strikes Back",
            "Interstellar"
        };
        var exampleVideos = fixture.GetVideoCollection(movieNames);
        await fixture.VideoPersistence.InsertList(exampleVideos);
        var input = new ListVideosInput(page, perPage, searchTerm);

        var (response, output) = await fixture.ApiClient
            .Get<TestApiResponseList<VideoModelOutput>>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(expectedTotalItems);
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(expectedReturnedtems);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        if (output.Data!.Any())
        {
            output.Data.Should().AllSatisfy(video => video.Title.Contains(searchTerm,
                StringComparison.CurrentCultureIgnoreCase));
        }
    }

    public void Dispose() => fixture.CleanPersistence();
}

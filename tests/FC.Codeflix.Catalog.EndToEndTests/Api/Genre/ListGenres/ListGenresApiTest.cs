using System.Net;
using FluentAssertions;
using FC.Codeflix.Catalog.Api.ApiModels.Response;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenres;
using FC.Codeflix.Catalog.EndToEndTests.Models;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.EndToEndTests.Extensions.DateTimes;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.ListGenres;

[Collection(nameof(ListGenresApiTestFixture))]
public class ListGenresApiTest: IDisposable
{
    private readonly ListGenresApiTestFixture _fixture;

    public ListGenresApiTest(ListGenresApiTestFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = nameof(ListGenre))]
    [Trait("EndToEnd/API", "Genre/List - Endpoints")]
    public async Task ListGenre()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(10);
        var targetGenre = exampleGenres[5];
        await _fixture.Persistence.InsertList(exampleGenres);

        var input = new ListGenresInput();
        input.Page = 1;
        input.PerPage = exampleGenres.Count;

        var (response, output) = await _fixture.ApiClient
            .Get<TestApiResponseList<GenreModelOutput>>("genres", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(exampleGenres.Count);
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(exampleGenres.Count);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data!.ForEach(outputItem =>
        {
            var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());
        });
    }

    [Fact(DisplayName = nameof(ListGenreWithRelations))]
    [Trait("EndToEnd/API", "Genre/List - Endpoints")]
    public async Task ListGenreWithRelations()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(15);
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoryList(10);
        Random random = new();
        exampleGenres.ForEach(genre =>
        {
            int relationsCount = random.Next(2, exampleCategories.Count - 1);
            for (int i = 0; i < relationsCount; i++)
            {
                int selectedCategoryIndex = random.Next(exampleCategories.Count - 1);
                DomainEntity.Category selected = exampleCategories[selectedCategoryIndex];
                if (!genre.Categories.Contains(selected.Id))
                    genre.AddCategory(selected.Id);
            }
        });
        List<GenresCategories> genresCategories = new();
        exampleGenres.ForEach(genre =>
            genre.Categories.ToList().ForEach(
                categoryId => genresCategories.Add(new GenresCategories(categoryId, genre.Id))
            )
        );
        await _fixture.Persistence.InsertList(exampleGenres);
        await _fixture.CategoryPersitence.InsertList(exampleCategories);
        await _fixture.Persistence.InsertGenresCategoriesRelationsList(genresCategories);
        var input = new ListGenresInput();
        input.Page = 1;
        input.PerPage = exampleGenres.Count;

        var (response, output) = await _fixture.ApiClient
            .Get<TestApiResponseList<GenreModelOutput>>("genres", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(exampleGenres.Count);
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(exampleGenres.Count);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data!.ForEach(outputItem =>
        {
            var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());
            var relatedCategoriesIds = outputItem.Categories.Select(x => x.Id).ToList();
            relatedCategoriesIds.Should().NotBeNull();
            relatedCategoriesIds.Should().BeEquivalentTo(exampleItem.Categories);
            outputItem.Categories.ToList().ForEach(outputRelatedCategory => {
                outputRelatedCategory.Should().NotBeNull();
                var exampleCategory = exampleCategories.Find(x => x.Id.Equals(outputRelatedCategory.Id));
                exampleCategory.Should().NotBeNull();
                outputRelatedCategory.Name.Should().Be(exampleCategory!.Name);
            });
        });
    }

    [Fact(DisplayName = nameof(EmptyWhenThereAreNotItems))]
    [Trait("EndToEnd/API", "Genre/List - Endpoints")]
    public async Task EmptyWhenThereAreNotItems()
    {
        
        var input = new ListGenresInput();
        input.Page = 1;
        input.PerPage = 10;

        var (response, output) = await _fixture.ApiClient
            .Get<TestApiResponseList<GenreModelOutput>>("genres", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(0);
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(0);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
    }

    [Theory(DisplayName = nameof(Paginated))]
    [Trait("EndToEnd/API", "Genre/List - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task Paginated(int quantityToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems)
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(quantityToGenerate);
        var targetGenre = exampleGenres[5];
        await _fixture.Persistence.InsertList(exampleGenres);

        var input = new ListGenresInput();
        input.Page = page;
        input.PerPage = perPage;

        var (response, output) = await _fixture.ApiClient
            .Get<TestApiResponseList<GenreModelOutput>>("genres", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(quantityToGenerate);
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(expectedQuantityItems);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data!.ForEach(outputItem =>
        {
            var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());
        });
    }
    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("EndToEnd/API", "Genre/List - Endpoints")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(string search, int page, int perPage, int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems)
    {
        var exampleGenresList = _fixture.GetExampleGenresListByNames(new List<string>()
        {
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Space",
            "Sci-fi Robots",
            "Sci-fi Future",
        });
        await _fixture.Persistence.InsertList(exampleGenresList);

        var input = new ListGenresInput();
        input.Page = page;
        input.PerPage = perPage;
        input.Search = search;

        var (response, output) = await _fixture.ApiClient
            .Get<TestApiResponseList<GenreModelOutput>>("genres", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(expectedQuantityTotalItems);
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(expectedQuantityItemsReturned);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data!.ForEach(outputItem =>
        {
            var exampleItem = exampleGenresList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());
        });
    }

    [Theory(DisplayName = nameof(Ordered))]
    [Trait("EndToEnd/API", "Genre/List - Endpoints")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "desc")]
    public async Task Ordered(string orderBy, string order)
    {
        var exampleGenresList = _fixture.GetExampleGenresList(10);
        await _fixture.Persistence.InsertList(exampleGenresList);

        var input = new ListGenresInput();
        input.Page = 1;
        input.PerPage = 10;
        input.Dir = order == "asc"? SearchOrder.Asc: SearchOrder.Desc;
        input.Sort = orderBy;

        var (response, output) = await _fixture.ApiClient
            .Get<TestApiResponseList<GenreModelOutput>>("genres", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(10);
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(10);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        var expectedOrderedList = _fixture.CloneGenresListOrdered(
              exampleGenresList,
              orderBy,
              input.Dir);
        
        for (int indice = 0; indice < expectedOrderedList.Count; indice++)
        {
            var outputItem = output.Data![indice];
            var exampleItem = exampleGenresList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());
        }

    }

    public void Dispose()
    {
        _fixture.CleanPersistence();
    }
}

using System.Net;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;



namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.ListCategorires;

[Collection(nameof(ListCategoryApiTestFixture))]
public class ListCategoriesApiTest : IDisposable
{
    private readonly ListCategoryApiTestFixture _fixture;

    public ListCategoriesApiTest(ListCategoryApiTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(ListCategoriesAndTotalByDefault))]
    [Trait("EndToEnd/API", "Category/GetList - Endpoints")]
    public async Task ListCategoriesAndTotalByDefault()
    {
        var defaultPerPage = 15;
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);

        var (response, output) = await _fixture.ApiClient.Get<ListCategoriesOutput>(
            "/categories");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Total.Should().Be(20);
        output!.Page.Should().Be(1);
        output!.PerPage.Should().Be(defaultPerPage);
        output.Items.Should().HaveCount(defaultPerPage);
        foreach (var outputItem in output.Items)
        {
            var expectedItem = exampleCategoriesList.First(x => x.Id == outputItem.Id);
            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
    }

    [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceEmpty))]
    [Trait("EndToEnd/API", "Category/GetList - Endpoints")]
    public async Task ItemsEmptyWhenPersistenceEmpty()
    {

        var (response, output) = await _fixture.ApiClient.Get<ListCategoriesOutput>(
            "/categories");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);

    }

    [Fact(DisplayName = nameof(ListCategoriesAndTotal))]
    [Trait("EndToEnd/API", "Category/GetList - Endpoints")]
    public async Task ListCategoriesAndTotal()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var input = new ListCategoriesInput(page: 1, perPage: 5);

        var (response, output) = await _fixture.ApiClient.Get<ListCategoriesOutput>(
            "/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(input.PerPage);
        output!.Page.Should().Be(input.Page);
        output!.PerPage.Should().Be(input.PerPage);

        foreach (var outputItem in output.Items)
        {
            var expectedItem = exampleCategoriesList.First(x => x.Id == outputItem.Id);
            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(ListPaginated))]
    [Trait("EndToEnd/API", "Category/GetList - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListPaginated(int quantityToGenerate, int page, int perPage,
        int expectedQuantityItems)
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(quantityToGenerate);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var input = new ListCategoriesInput(page, perPage);

        var (response, output) = await _fixture.ApiClient.Get<ListCategoriesOutput>(
            "/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Total.Should().Be(quantityToGenerate);
        output.Items.Should().HaveCount(expectedQuantityItems);
        output!.Page.Should().Be(page);
        output!.PerPage.Should().Be(perPage);

        foreach (var outputItem in output.Items)
        {
            var expectedItem = exampleCategoriesList.First(x => x.Id == outputItem.Id);
            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
    }
    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("EndToEnd/API", "Category/GetList - Endpoints")]
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
        var categoryNamesList = new List<string>
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
        };
        var exampleCategoriesList = _fixture.GetExampleCategoriesListWithName(categoryNamesList);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var input = new ListCategoriesInput(page, perPage, search);

        var (response, output) = await _fixture.ApiClient.Get<ListCategoriesOutput>(
            "/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Total.Should().Be(expectedQuantityTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        output!.Page.Should().Be(page);
        output!.PerPage.Should().Be(perPage);

        foreach (var outputItem in output.Items)
        {
            var expectedItem = exampleCategoriesList.First(x => x.Id == outputItem.Id);
            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("EndToEnd/API", "Category/GetList - Endpoints")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "desc")]
    public async Task SearchOrdered(string orderBy, string order)
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var inputOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var input = new ListCategoriesInput(1, 20, 
            sort: orderBy, 
            dir: inputOrder);

        var (response, output) = await _fixture.ApiClient.Get<ListCategoriesOutput>(
            "/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(exampleCategoriesList.Count);
        output!.Page.Should().Be(input.Page);
        output!.PerPage.Should().Be(input.PerPage);
        var orderedItems = _fixture.CloneCategoriesListOrdered(exampleCategoriesList, input.Sort, input.Dir);
        for (int i = 0; i < orderedItems.Count; i++)
        {
            var expectedItem = orderedItems[i];
            var outputItem = output.Items[i];

            outputItem.Should().NotBeNull();
            expectedItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(expectedItem!.Name);
            outputItem!.Id.Should().Be(expectedItem!.Id);
            outputItem.Description.Should().Be(expectedItem?.Description);
            outputItem.IsActive.Should().Be(expectedItem!.IsActive);
            outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
    }
    public void Dispose()
        => _fixture.CleanPersistence();
}

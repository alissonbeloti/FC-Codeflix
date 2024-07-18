using System.Net;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.EndToEndTests.Extensions.DateTimes;
using Xunit.Abstractions;
using Newtonsoft.Json;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.EndToEndTests.Models;



namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.ListCategorires;



[Collection(nameof(ListCategoryApiTestFixture))]
public class ListCategoriesApiTest : IDisposable
{
    private readonly ListCategoryApiTestFixture _fixture;
    private readonly ITestOutputHelper _output;
    public ListCategoriesApiTest(ListCategoryApiTestFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    [Fact(DisplayName = nameof(ListCategoriesAndTotalByDefault))]
    [Trait("EndToEnd/API", "Category/GetList - Endpoints")]
    public async Task ListCategoriesAndTotalByDefault()
    {
        var defaultPerPage = 15;
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);

        var (response, output) = await _fixture.ApiClient.Get<CategoryListOutput>(
            "/categories");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output!.Data!.Should().NotBeNull();
        output.Meta.Total.Should().Be(20);
        output.Meta.CurrentPage.Should().Be(1);
        output.Meta.PerPage.Should().Be(defaultPerPage);
        output.Data.Should().HaveCount(defaultPerPage);
        foreach (var outputItem in output.Data)
        {
            var expectedItem = exampleCategoriesList.First(x => x.Id == outputItem.Id);
            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.TrimMilliseconds().Should().Be(expectedItem.CreatedAt.TrimMilliseconds());
        }
    }

    [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceEmpty))]
    [Trait("EndToEnd/API", "Category/GetList - Endpoints")]
    public async Task ItemsEmptyWhenPersistenceEmpty()
    {

        var (response, output) = await _fixture.ApiClient.Get<CategoryListOutput>(
            "/categories");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Total.Should().Be(0);
        output.Data.Should().HaveCount(0);

    }

    [Fact(DisplayName = nameof(ListCategoriesAndTotal))]
    [Trait("EndToEnd/API", "Category/GetList - Endpoints")]
    public async Task ListCategoriesAndTotal()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var input = new ListCategoriesInput(page: 1, perPage: 5);

        var (response, output) = await _fixture.ApiClient.Get<CategoryListOutput>(
            "/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Total.Should().Be(exampleCategoriesList.Count);
        output.Data.Should().HaveCount(input.PerPage);
        output!.Meta.CurrentPage.Should().Be(input.Page);
        output!.Meta.PerPage.Should().Be(input.PerPage);

        foreach (var outputItem in output.Data)
        {
            var expectedItem = exampleCategoriesList.First(x => x.Id == outputItem.Id);
            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.TrimMilliseconds().Should().Be(expectedItem.CreatedAt.TrimMilliseconds());
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

        var (response, output) = await _fixture.ApiClient.Get<CategoryListOutput>(
            "/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Total.Should().Be(quantityToGenerate);
        output.Data.Should().HaveCount(expectedQuantityItems);
        output!.Meta.CurrentPage.Should().Be(page);
        output!.Meta.PerPage.Should().Be(perPage);

        foreach (var outputItem in output.Data)
        {
            var expectedItem = exampleCategoriesList.First(x => x.Id == outputItem.Id);
            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.TrimMilliseconds().Should().Be(expectedItem.CreatedAt.TrimMilliseconds());
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

        var (response, output) = await _fixture.ApiClient.Get<CategoryListOutput>(
            "/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Total.Should().Be(expectedQuantityTotalItems);
        output.Data.Should().HaveCount(expectedQuantityItemsReturned);
        output!.Meta.CurrentPage.Should().Be(page);
        output!.Meta.PerPage.Should().Be(perPage);

        foreach (var outputItem in output.Data)
        {
            var expectedItem = exampleCategoriesList.First(x => x.Id == outputItem.Id);
            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.TrimMilliseconds().Should().Be(expectedItem.CreatedAt.TrimMilliseconds());
        }
    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("EndToEnd/API", "Category/GetList - Endpoints")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("", "desc")]
    public async Task SearchOrdered(string orderBy, string order)
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var inputOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var input = new ListCategoriesInput(1, 20, 
            sort: orderBy, 
            dir: inputOrder);

        var (response, output) = await _fixture.ApiClient.Get<CategoryListOutput>(
            "/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Total.Should().Be(exampleCategoriesList.Count);
        output.Data.Should().HaveCount(exampleCategoriesList.Count);
        output!.Meta.CurrentPage.Should().Be(input.Page);
        output!.Meta.PerPage.Should().Be(input.PerPage);
        var orderedItems = _fixture.CloneCategoriesListOrdered(exampleCategoriesList, input.Sort, input.Dir);
        var count = 0;
        var expectedArray = orderedItems.Select(x => $"{++count} {x.Name} {x.CreatedAt} {JsonConvert.SerializeObject(x)}").ToArray();
        count = 0;
        var outputArr = output.Data.Select(x => $"{++count} {x.Name} {x.CreatedAt} {JsonConvert.SerializeObject(x)}").ToArray();

        _output.WriteLine("Expecteds...");
        _output.WriteLine(string.Join("\n", expectedArray));
        _output.WriteLine("Outputs (actual)...");
        _output.WriteLine(string.Join("\n", outputArr));
        for (int i = 0; i < orderedItems.Count; i++)
        {
            var expectedItem = orderedItems[i];
            var outputItem = output.Data[i];

            outputItem.Should().NotBeNull();
            expectedItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(expectedItem!.Name);
            outputItem!.Id.Should().Be(expectedItem!.Id);
            outputItem.Description.Should().Be(expectedItem?.Description);
            outputItem.IsActive.Should().Be(expectedItem!.IsActive);
            outputItem.CreatedAt.TrimMilliseconds().Should().Be(expectedItem.CreatedAt.TrimMilliseconds());
        }
    }

    [Theory(DisplayName = nameof(SearchOrderedDates))]
    [Trait("EndToEnd/API", "Category/GetList - Endpoints")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    public async Task SearchOrderedDates(string orderBy, string order)
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var inputOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var input = new ListCategoriesInput(1, 20,
            sort: orderBy,
            dir: inputOrder);

        var (response, output) = await _fixture.ApiClient.Get<CategoryListOutput>(
            "/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Total.Should().Be(exampleCategoriesList.Count);
        output.Data.Should().HaveCount(exampleCategoriesList.Count);
        output!.Meta.CurrentPage.Should().Be(input.Page);
        output!.Meta.PerPage.Should().Be(input.PerPage);
        DateTime? lastDateItemDate = null;

        var count = 0;
        var outputArr = output.Data.Select(x => $"{++count} {x.Name} {x.CreatedAt} {JsonConvert.SerializeObject(x)}").ToArray();
        _output.WriteLine("Outputs (actual)...");
        _output.WriteLine(string.Join("\n", outputArr));

        foreach (var outputItem in output.Data)
        {
            var expectedItem = exampleCategoriesList.First(x => x.Id == outputItem.Id);
            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.TrimMilliseconds().Should().Be(expectedItem.CreatedAt.TrimMilliseconds());
            if (lastDateItemDate.HasValue)
            {
                if (order == "asc")
                {
                    Assert.True(outputItem.CreatedAt >= lastDateItemDate);
                }
                else
                {
                    Assert.True(outputItem.CreatedAt <= lastDateItemDate);
                }
            }
            lastDateItemDate = outputItem.CreatedAt;
        }
    }
    public void Dispose()
        => _fixture.CleanPersistence();
}

using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.Category.GetCategory;
using FC.Codeflix.Catalog.EndToEndTests.Extensions.DateTimes;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.GetCategoryById;


[Collection(nameof(GetCategoryApiTestFixture))]
public class GetCategoryApiTest: IDisposable
{
    private readonly GetCategoryApiTestFixture _fixture;

    public GetCategoryApiTest(GetCategoryApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetCategory))]
    [Trait("EndToEnd/API", "Category/Get - Endpoints")]
    public async Task GetCategory()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];

        var (response, output) = await _fixture.ApiClient.Get<ApiResponse<CategoryModelOutput>>(
            $"/categories/{exampleCategory.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(exampleCategory.Id);
        output.Data.Name.Should().Be(exampleCategory.Name);
        output.Data.Description.Should().Be(exampleCategory.Description);
        output.Data.IsActive.Should().Be(exampleCategory.IsActive);
        output.Data.CreatedAt.TrimMilliseconds().Should().Be(exampleCategory.CreatedAt.TrimMilliseconds());
    }

    [Fact(DisplayName = nameof(ErrorWhenNotFound))]
    [Trait("EndToEnd/API", "Category/Get - Endpoints")]
    public async Task ErrorWhenNotFound()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleGuid = Guid.NewGuid();

        var (response, output) = await _fixture.ApiClient.Get<ProblemDetails>(
            $"/categories/{exampleGuid}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be(404);
        output.Type.Should().Be("NotFound");
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"Category '{exampleGuid}' not found.");
    }
    public void Dispose()
        => _fixture.CleanPersistence();
}

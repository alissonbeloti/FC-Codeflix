using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.DeleteCategory;
[Collection(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryApiTest: IDisposable
{
    private readonly DeleteCategoryTestFixture _fixture;

    public DeleteCategoryApiTest(DeleteCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteCategory))]
    [Trait("EndToEnd/API", "Category/Delete - Endpoints")]
    public async Task DeleteCategory()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];

        var (response, output) = await _fixture.ApiClient.Delete<object>(
            $"/categories/{exampleCategory.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var persistenceCategory = await _fixture.Persistence
            .GetById(exampleCategory.Id);
        persistenceCategory.Should().BeNull();
    }

    [Fact(DisplayName = nameof(ErrorWhenNotFound))]
    [Trait("EndToEnd/API", "Category/Delete - Endpoints")]
    public async Task ErrorWhenNotFound()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var ramdomGuid = Guid.NewGuid();

        var (response, output) = await _fixture.ApiClient.Delete<ProblemDetails>(
            $"/categories/{ramdomGuid}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Title.Should().Be("Not Found");
        output.Type.Should().Be("NotFound");
        output.Status.Should().Be((int)HttpStatusCode.NotFound);
        output.Detail.Should().Be($"Category '{ramdomGuid}' not found.");
    }
    public void Dispose()
        => _fixture.CleanPersistence();
}

using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FC.Codeflix.Catalog.Api.ApiModels.Category;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Api.ApiModels.Response;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory;


[Collection(nameof(UpdateApiCategoryTestFixture))]
public class UpdateApiCategoryTest: IDisposable
{
    private readonly UpdateApiCategoryTestFixture _fixture;

    public UpdateApiCategoryTest(UpdateApiCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(UpdateCategory))]
    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    public async Task UpdateCategory()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];
        var input = _fixture.GetExampleInput();
        var (response, output) = await _fixture.ApiClient.Put<ApiResponse<CategoryModelOutput>>(
            $"/categories/{exampleCategory.Id}", 
            input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(exampleCategory.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.Description.Should().Be(input.Description);
        output.Data.IsActive.Should().Be(input.IsActive!.Value);

        DomainEntity.Category? dbCategory = await _fixture.Persistence
            .GetById(output.Data.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Id.Should().NotBeEmpty();
        dbCategory.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(input.IsActive!.Value);

    }

    [Fact(DisplayName = nameof(UpdateCategoryOnlyName))]
    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    public async Task UpdateCategoryOnlyName()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];
        var input = new UpdateCategoryApiInput(_fixture.GetValidCategoryName());


        var (response, output) = await _fixture.ApiClient.Put<ApiResponse<CategoryModelOutput>>(
            $"/categories/{exampleCategory.Id}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(exampleCategory.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.Description.Should().Be(exampleCategory.Description);
        output.Data.IsActive.Should().Be(exampleCategory.IsActive);

        DomainEntity.Category? dbCategory = await _fixture.Persistence
            .GetById(output.Data.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Id.Should().NotBeEmpty();
        dbCategory.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);

    }

    [Fact(DisplayName = nameof(UpdateCategoryNameAndDescription))]
    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    public async Task UpdateCategoryNameAndDescription()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];
        var input = new UpdateCategoryApiInput(_fixture.GetValidCategoryName(),
            _fixture.GetValidCategoryDescription());


        var (response, output) = await _fixture.ApiClient.Put<ApiResponse<CategoryModelOutput>>(
            $"/categories/{exampleCategory.Id}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(exampleCategory.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.Description.Should().Be(input.Description);
        output.Data.IsActive.Should().Be(exampleCategory.IsActive);

        DomainEntity.Category? dbCategory = await _fixture.Persistence
            .GetById(output.Data.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Id.Should().NotBeEmpty();
        dbCategory.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);

    }

    [Fact(DisplayName = nameof(ErrorWhenNotFound))]
    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    public async Task ErrorWhenNotFound()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var randomGuid = Guid.NewGuid();
        var input = _fixture.GetExampleInput();

        var (response, output) = await _fixture.ApiClient.Put<ProblemDetails>(
            $"/categories/{randomGuid}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Title.Should().Be("Not Found");
        output.Type.Should().Be("NotFound");
        output.Status.Should().Be((int)HttpStatusCode.NotFound);
        output.Detail.Should().Be($"Category '{randomGuid}' not found.");

    }

    [Theory(DisplayName = nameof(ErrorWhenCantInstatiateAggregate))]
    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    [MemberData(nameof(UpdateCategoryApiTestDataGenerator.GetInvalidInputs),
        MemberType = typeof(UpdateCategoryApiTestDataGenerator))]
    public async Task ErrorWhenCantInstatiateAggregate(UpdateCategoryApiInput input, string expectedDetail)
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];

        var (response, output) = await _fixture.ApiClient.Put<ProblemDetails>(
            $"/categories/{exampleCategory.Id}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Title.Should().Be("One or more validation errors occurred");
        output.Type.Should().Be("UnprocessableEntity");
        output.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        output.Detail.Should().Be(expectedDetail);
    }


    public void Dispose()
        => _fixture.CleanPersistence();
}

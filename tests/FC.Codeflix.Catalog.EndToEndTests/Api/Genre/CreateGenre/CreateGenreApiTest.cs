using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.Codeflix.Catalog.Domain.Entity;
using FluentAssertions;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.CreateGenre;

[Collection(nameof(CreateGenreApiTestFixture))]
public class CreateGenreApiTest
{
    private readonly CreateGenreApiTestFixture _fixture;

    public CreateGenreApiTest(CreateGenreApiTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("EndToEnd/API", "Genre/Create - Endpoints")]
    public async Task CreateGenre()
    {
        var apiInput = new CreateGenreInput(
                _fixture.GetValidGenreName(),
                _fixture.GetRandoBoolean()
                );

        var (response, output) = await _fixture.ApiClient
            .Post<ApiResponse<GenreModelOutput>>($"/genres", apiInput);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(apiInput.Name);
        output.Data.IsActive.Should().Be(apiInput.IsActive);
        output.Data.Categories.Should().HaveCount(0);
        var genreDb = await _fixture.Persistence.GetById(output.Data.Id);
        genreDb.Should().NotBeNull();
        genreDb!.Name.Should().Be(apiInput.Name);
        genreDb!.IsActive.Should().Be(apiInput.IsActive);
    }

    [Fact(DisplayName = nameof(CreateGenreWithRelations))]
    [Trait("EndToEnd/API", "Genre/Create - Endpoints")]
    public async Task CreateGenreWithRelations()
    {
        var exampleCategories = _fixture.GetExampleCategoryList(10);
        await _fixture.CategoryPersitence.InsertList(exampleCategories);
        var relatedCategories = exampleCategories.Skip(3).Take(3).Select(x => x.Id).ToList();
        var apiInput = new CreateGenreInput(
                _fixture.GetValidGenreName(),
                _fixture.GetRandoBoolean(),
                relatedCategories
                );

        var (response, output) = await _fixture.ApiClient
            .Post<ApiResponse<GenreModelOutput>>($"/genres", apiInput);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(apiInput.Name);
        output.Data.IsActive.Should().Be(apiInput.IsActive);
        output.Data.Categories.Should().HaveCount(relatedCategories.Count);
        var outputRelatedCategoriesIds = output.Data.Categories.Select(x => x.Id).ToList();
        outputRelatedCategoriesIds.Should().BeEquivalentTo(relatedCategories);
        var genreDb = await _fixture.Persistence.GetById(output.Data.Id);
        genreDb.Should().NotBeNull();
        genreDb!.Name.Should().Be(apiInput.Name);
        genreDb!.IsActive.Should().Be(apiInput.IsActive);
        var relationsFromDb = 
            await _fixture.Persistence.GetGenresCategoriesRelationsByGenreId(output.Data.Id); 
        relationsFromDb.Should().NotBeNull();
        relationsFromDb.Should().HaveCount(relatedCategories.Count);
        var relatedCategoriesIdsFromDb = relationsFromDb.Select(x => x.CategoryId).ToList();
        relatedCategoriesIdsFromDb.Should().BeEquivalentTo(relatedCategories);
    }

    [Fact(DisplayName = nameof(CreateGenreWithRelations))]
    [Trait("EndToEnd/API", "Genre/Create - Endpoints")]
    public async Task ErrorWithInvalidRelations()
    {
        var exampleCategories = _fixture.GetExampleCategoryList(10);
        await _fixture.CategoryPersitence.InsertList(exampleCategories);
        var relatedCategories = exampleCategories.Skip(3).Take(3).Select(x => x.Id).ToList();
        var invalidCategoryId = Guid.NewGuid();
        var apiInput = new CreateGenreInput(
                _fixture.GetValidGenreName(),
                _fixture.GetRandoBoolean(),
                relatedCategories
                );
        relatedCategories.Add(invalidCategoryId);
        var (response, output) = await _fixture.ApiClient
            .Post<ProblemDetails>($"/genres", apiInput);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(System.Net.HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related category id (or ids) not found: {invalidCategoryId}.");
    }
}

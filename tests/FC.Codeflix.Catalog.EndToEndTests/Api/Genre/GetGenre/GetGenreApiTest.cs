using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using System.Net;

using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.GetGenre;

[Collection(nameof(GetGenreApiTestFixture))]
public class GetGenreApiTest : IDisposable
{
    private readonly GetGenreApiTestFixture _fixture;

    public GetGenreApiTest(GetGenreApiTestFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("EndToEnd/API", "Genre/Get - Endpoints")]
    public async Task GetGenre()
    {
        // Cadastrar lista de genres na persistência
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(10);
        var targetGenre = exampleGenres[5];
        await _fixture.GenrePersistence.InsertList(exampleGenres);

        // buscar por genre específico por rest
        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}");

        // testa e ve se o item ó que se espera
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output!.Data.Name.Should().Be(targetGenre.Name);
        output!.Data.IsActive.Should().Be(targetGenre.IsActive);
    }

    [Fact(DisplayName = nameof(NotFound))]
    [Trait("EndToEnd/API", "Genre/Get - Endpoints")]
    public async Task NotFound()
    {
        // Cadastrar lista de genres na persistência
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(10);
        var randomGuid = Guid.NewGuid();
        await _fixture.GenrePersistence.InsertList(exampleGenres);

        // buscar por genre específico por rest
        var (response, output) = await _fixture.ApiClient
            .Get<ProblemDetails>($"/genres/{randomGuid}");

        // testa e ve se o item é o que se espera
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Type.Should().Be("NotFound");
        output!.Detail.Should().Be($"Genre '{randomGuid}' not found.");
    }

    [Fact(DisplayName = nameof(GetGenreWithRelations))]
    [Trait("EndToEnd/API", "Genre/Get - Endpoints")]
    public async Task GetGenreWithRelations()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(10);
        var targetGenre = exampleGenres[5];
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoryList(10);
        Random random = new();
        exampleGenres.ForEach(genre =>
        {
            int relationsCount = random.Next(2, exampleCategories.Count-1);
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
        
        await _fixture.GenrePersistence.InsertList(exampleGenres);
        await _fixture.CategoryPersitence.InsertList(exampleCategories);
        await _fixture.GenrePersistence.InsertGenresCategoriesRelationsList(genresCategories);

        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output!.Data.Name.Should().Be(targetGenre.Name);
        output!.Data.IsActive.Should().Be(targetGenre.IsActive);
        List<Guid> relatedCategoriesIds = 
            output.Data.Categories.Select(relation => relation.Id).ToList();
        relatedCategoriesIds.Should().BeEquivalentTo(targetGenre.Categories);
    }

    public void Dispose() => _fixture.CleanPersistence();
}

using System.Net;
using FluentAssertions;
using FC.Codeflix.Catalog.Api.ApiModels.Response;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using Microsoft.AspNetCore.Mvc;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.DeleteGenre;

[Collection(nameof(DeleteGenreApiTestFixture))]
public class DeleteGenreApiTest : IDisposable
{
    private readonly DeleteGenreApiTestFixture _fixture;

    public DeleteGenreApiTest(DeleteGenreApiTestFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("EndToEnd/API", "Genre/Delete - Endpoints")]
    public async Task DeleteGenre()
    {

        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(10);
        var targetGenre = exampleGenres[5];
        await _fixture.GenrePersistence.InsertList(exampleGenres);

        var (response, output) = await _fixture.ApiClient
            .Delete<object>($"/genres/{targetGenre.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        DomainEntity.Genre? genreDb = await _fixture.GenrePersistence.GetById(targetGenre.Id);
        genreDb.Should().BeNull();
    }

    [Fact(DisplayName = nameof(WhenNotFound404))]
    [Trait("EndToEnd/API", "Genre/Delete - Endpoints")]
    public async Task WhenNotFound404()
    {

        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(10);
        var randomGuid = Guid.NewGuid();
        await _fixture.GenrePersistence.InsertList(exampleGenres);

        var (response, output) = await _fixture.ApiClient
            .Delete<ProblemDetails>($"/genres/{randomGuid}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Type.Should().Be("NotFound");
        output!.Detail.Should().Be($"Genre '{randomGuid}' not found.");
    }

    [Fact(DisplayName = nameof(DeleteGenreWithRelations))]
    [Trait("EndToEnd/API", "Genre/Delete - Endpoints")]    
    public async Task DeleteGenreWithRelations()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(10);
        var targetGenre = exampleGenres[5];
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

        await _fixture.GenrePersistence.InsertList(exampleGenres);
        await _fixture.CategoryPersitence.InsertList(exampleCategories);
        await _fixture.GenrePersistence.InsertGenresCategoriesRelationsList(genresCategories);

        var (response, output) = await _fixture.ApiClient
            .Delete<object>($"/genres/{targetGenre.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        DomainEntity.Genre? genreDb = await _fixture.GenrePersistence.GetById(targetGenre.Id);
        genreDb.Should().BeNull();
        List<GenresCategories> relations = await _fixture.GenrePersistence.GetGenresCategoriesRelationsByGenreId(targetGenre.Id);
        relations.Should().HaveCount(0);
    }

    public void Dispose() => _fixture.CleanPersistence();
}

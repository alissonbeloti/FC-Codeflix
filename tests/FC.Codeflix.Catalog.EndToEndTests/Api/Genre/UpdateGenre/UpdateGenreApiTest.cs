﻿using System.Net;
using FluentAssertions;
using FC.Codeflix.Catalog.Api.ApiModels.Response;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Api.ApiModels.Genre;
using Microsoft.AspNetCore.Mvc;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.UpdateGenre;

[Collection(nameof(UpdateGenreApiTestFixture))]
public class UpdateGenreApiTest : IDisposable
{
    private readonly UpdateGenreApiTestFixture _fixture;

    public UpdateGenreApiTest(UpdateGenreApiTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("EndToEnd/API", "Genre/Update - Endpoints")]
    public async Task UpdateGenre ()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(10);
        var targetGenre = exampleGenres[5];
        await _fixture.GenrePersistence.InsertList(exampleGenres);
        var input = new UpdateGenreApiInput(_fixture.GetValidGenreName(), _fixture.GetRandomBoolean());
        // buscar por genre específico por rest
        var (response, output) = await _fixture.ApiClient
            .Put<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}", input);

        // testa e ve se o item ó que se espera
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output!.Data.Name.Should().Be(input.Name);
        output!.Data.IsActive.Should().Be(input.IsActive!.Value);
        var genreDb = await _fixture.GenrePersistence.GetById(output.Data.Id);
        genreDb.Should().NotBeNull();
        genreDb!.Name.Should().Be(input.Name);
        genreDb!.IsActive.Should().Be(input.IsActive!.Value);
    }

    [Fact(DisplayName = nameof(ErrorWhenInvalidRelation))]
    [Trait("EndToEnd/API", "Genre/Update - Endpoints")]
    public async Task ErrorWhenInvalidRelation()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(10);
        var targetGenre = exampleGenres[5];
        var invalidRelatedId = Guid.NewGuid();
        await _fixture.GenrePersistence.InsertList(exampleGenres);
        var input = new UpdateGenreApiInput(_fixture.GetValidGenreName(), _fixture.GetRandomBoolean(),
            new List<Guid> { invalidRelatedId });
        // buscar por genre específico por rest
        var (response, output) = await _fixture.ApiClient
            .Put<ProblemDetails>($"/genres/{targetGenre.Id}", input);

        // testa e ve se o item ó que se espera
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related category id (or ids) not found: {invalidRelatedId}.");
    }

    [Fact(DisplayName = nameof(ProblemDetailsWhenNotFound))]
    [Trait("EndToEnd/API", "Genre/Update - Endpoints")]
    public async Task ProblemDetailsWhenNotFound()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(10);
        var randomGuid = Guid.NewGuid();
        await _fixture.GenrePersistence.InsertList(exampleGenres);
        var input = new UpdateGenreApiInput(_fixture.GetValidGenreName(), _fixture.GetRandomBoolean());
        // buscar por genre específico por rest
        var (response, output) = await _fixture.ApiClient
            .Put<ProblemDetails>($"/genres/{randomGuid}", input);

        // testa e ve se o item ó que se espera
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Title.Should().Be("Not Found"); 
        output.Detail.Should().Be($"Genre '{randomGuid}' not found.");
        output.Type.Should().Be("NotFound");
        output.Status.Should().Be(404);
        
    }

    [Fact(DisplayName = nameof(UpdateGenreWithRelations))]
    [Trait("EndToEnd/API", "Genre/Update - Endpoints")]
    public async Task UpdateGenreWithRelations()
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
        var newRelationsCount = random.Next(2, exampleCategories.Count - 1);
        var newRelatedCategoriesIds = new List<Guid>();
        for (int i = 0; i < newRelationsCount; i++)
        {
            int selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
            DomainEntity.Category selected = exampleCategories[selectedCategoryIndex];
            if (!newRelatedCategoriesIds.Contains(selected.Id))
                newRelatedCategoriesIds.Add(selected.Id);
        }
        await _fixture.GenrePersistence.InsertList(exampleGenres);
        await _fixture.CategoryPersitence.InsertList(exampleCategories);
        await _fixture.GenrePersistence.InsertGenresCategoriesRelationsList(genresCategories);
        var input = new UpdateGenreApiInput(_fixture.GetValidGenreName(), _fixture.GetRandomBoolean(),
            newRelatedCategoriesIds
            );

        var (response, output) = await _fixture.ApiClient
            .Put<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output!.Data.Name.Should().Be(input.Name);
        output!.Data.IsActive.Should().Be(input.IsActive!.Value);
        List<Guid> relatedCategoriesIdsFromOutput =
            output.Data.Categories.Select(relation => relation.Id).ToList();
        relatedCategoriesIdsFromOutput.Should().BeEquivalentTo(newRelatedCategoriesIds);
        var genreDb = await _fixture.GenrePersistence.GetById(output.Data.Id);
        genreDb.Should().NotBeNull();
        genreDb!.Name.Should().Be(input.Name);
        genreDb!.IsActive.Should().Be(input.IsActive!.Value);
        var genresCategoriesFromDb = await _fixture.GenrePersistence.
            GetGenresCategoriesRelationsByGenreId(genreDb.Id);
        var relatedCategoriesIdsFromDb = genresCategoriesFromDb.Select(x => x.CategoryId).ToList();
        relatedCategoriesIdsFromDb.Should().BeEquivalentTo(newRelatedCategoriesIds);
    }
    
    [Fact(DisplayName = nameof(PersistsRelationsWhenNotPresentInInput))]
    [Trait("EndToEnd/API", "Genre/Update - Endpoints")]
    public async Task PersistsRelationsWhenNotPresentInInput()
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
        var input = new UpdateGenreApiInput(_fixture.GetValidGenreName(), _fixture.GetRandomBoolean());

        var (response, output) = await _fixture.ApiClient
            .Put<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output!.Data.Name.Should().Be(input.Name);
        output!.Data.IsActive.Should().Be(input.IsActive!.Value);
        List<Guid> relatedCategoriesIdsFromOutput =
            output.Data.Categories.Select(relation => relation.Id).ToList();
        relatedCategoriesIdsFromOutput.Should().BeEquivalentTo(targetGenre.Categories);
        var genreDb = await _fixture.GenrePersistence.GetById(output.Data.Id);
        genreDb.Should().NotBeNull();
        genreDb!.Name.Should().Be(input.Name);
        genreDb!.IsActive.Should().Be(input.IsActive!.Value);
        var genresCategoriesFromDb = await _fixture.GenrePersistence.
            GetGenresCategoriesRelationsByGenreId(genreDb.Id);
        var relatedCategoriesIdsFromDb = genresCategoriesFromDb.Select(x => x.CategoryId).ToList();
        relatedCategoriesIdsFromDb.Should().BeEquivalentTo(targetGenre.Categories);
    }

    public void Dispose()
    {
        _fixture.CleanPersistence();
    }
}

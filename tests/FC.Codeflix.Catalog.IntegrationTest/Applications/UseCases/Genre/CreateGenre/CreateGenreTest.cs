using FluentAssertions;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using ApplicationUseCases = FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Application.Exceptions;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Genre.CreateGenre;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTest
{
    private readonly CreateGenreTestFixture _fixture;

    public CreateGenreTest(CreateGenreTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(CreateGenreOK))]
    [Trait("Integration/Application", "Use Cases - CreateGenre")]
    private async Task CreateGenreOK()
    {
        ApplicationUseCases.CreateGenreInput input = _fixture.GetExampleInput();
        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext();
        GenreRepository genreRepository = new(actDbContext);
        CategoryRepository categoryRepository = new(actDbContext);
        UnitOfWork unitOfWork = new(actDbContext);
        ApplicationUseCases.CreateGenre useCase = new(genreRepository, unitOfWork, categoryRepository);

        GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Categories.Should().HaveCount(0);

        DomainEntity.Genre? dbGenre = await (_fixture.CreateDbContext(true)).Genres.FindAsync(output.Id);

        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(input.Name);
        dbGenre.IsActive.Should().Be(input!.IsActive);
        dbGenre.CreatedAt.Should().Be(output.CreatedAt);
       
    }

    [Fact(DisplayName = nameof(CreateGenreWithCategoryRelations))]
    [Trait("Integration/Application", "Use Cases - CreateGenre")]
    private async Task CreateGenreWithCategoryRelations()
    {
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoryList(5);
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeDbContext.SaveChangesAsync();
        ApplicationUseCases.CreateGenreInput input = _fixture.GetExampleInput();
        input.CategoriesIds = exampleCategories
            .Select(category => category.Id)
            .ToList(); ;
        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        GenreRepository genreRepository = new(actDbContext);
        CategoryRepository categoryRepository = new(actDbContext);
        UnitOfWork unitOfWork = new(actDbContext);
        ApplicationUseCases.CreateGenre useCase = new(genreRepository, unitOfWork, categoryRepository);

        GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Categories.Should().HaveCount(input.CategoriesIds.Count);
        output.Categories.Select(relation => relation.Id).Should().BeEquivalentTo(input.CategoriesIds);
        //Assertion in DB
        CodeflixCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
        DomainEntity.Genre? dbGenre = await assertDbContext.Genres.FindAsync(output.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(input.Name);
        dbGenre.IsActive.Should().Be(input!.IsActive);
        dbGenre.CreatedAt.Should().Be(output.CreatedAt);
        List<GenresCategories> relations =  await assertDbContext.GenresCategories.AsNoTracking()
            .Where(x => x.GenreId == output.Id).ToListAsync();
        relations.Should().HaveCount(input.CategoriesIds.Count);
        List<Guid> idsRelated = relations.Select(x => x.CategoryId).ToList();
        idsRelated.Should().BeEquivalentTo(input.CategoriesIds);
    }

    [Fact(DisplayName = nameof(CreateGenreThrowsWhenCategoryDoesExists))]
    [Trait("Integration/Application", "Use Cases - CreateGenre")]
    private async Task CreateGenreThrowsWhenCategoryDoesExists()
    {
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoryList(5);
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeDbContext.SaveChangesAsync();
        ApplicationUseCases.CreateGenreInput input = _fixture.GetExampleInput();
        input.CategoriesIds = exampleCategories
            .Select(category => category.Id)
            .ToList();
        Guid randomGuid = Guid.NewGuid();
        input.CategoriesIds.Add(randomGuid);
        
        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        GenreRepository genreRepository = new(actDbContext);
        CategoryRepository categoryRepository = new(actDbContext);
        UnitOfWork unitOfWork = new(actDbContext);
        ApplicationUseCases.CreateGenre useCase = new(genreRepository, unitOfWork, categoryRepository);

        Func<Task<GenreModelOutput>> action = async () => await useCase
            .Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {randomGuid}.");
    }
}

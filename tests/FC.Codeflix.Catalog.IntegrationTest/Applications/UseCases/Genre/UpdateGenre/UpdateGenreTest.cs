using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using FluentAssertions;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Genre.UpdateGenre;

[Collection(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTest
{
    private readonly UpdateGenreTestFixture _fixture;

    public UpdateGenreTest(UpdateGenreTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("Integration/Application", "Use Cases - UpdateGenre")]
    public async Task UpdateGenre()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(10);
        CodeflixCatalogDbContext arrangeContext = _fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        await arrangeContext.AddRangeAsync(exampleGenres);
        await arrangeContext.SaveChangesAsync();

        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(actDbContext, eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());
        UseCase.UpdateGenre updateGenre = new(
                                            new GenreRepository(actDbContext), 
                                            unitOfWork,
                                            new CategoryRepository(actDbContext));
        UseCase.UpdateGenreInput input = new(targetGenre.Id,
            _fixture.GetValidGenreName(),
            !targetGenre.IsActive);
        GenreModelOutput output = await updateGenre.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive!.Value);
        CodeflixCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
        DomainEntity.Genre? genreDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreDb.Should().NotBeNull();
        genreDb!.Id.Should().Be(targetGenre.Id);
        genreDb.Name.Should().Be(input.Name);
        genreDb.IsActive.Should().Be(input.IsActive!.Value);
    }

    [Fact(DisplayName = nameof(UpdateGenreWithCategoriesRelations))]
    [Trait("Integration/Application", "Use Cases - UpdateGenre")]
    public async Task UpdateGenreWithCategoriesRelations()
    {
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoryList(10);
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(10);
        CodeflixCatalogDbContext arrangeContext = _fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        List<DomainEntity.Category> relatedCategories = exampleCategories.GetRange(0, 5);
        List<DomainEntity.Category> newRelatedCategories = exampleCategories.GetRange(5, 3);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        List<GenresCategories> relations = targetGenre.Categories
            .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id))
            .ToList();
        await arrangeContext.AddRangeAsync(exampleGenres);
        await arrangeContext.AddRangeAsync(exampleCategories);
        await arrangeContext.AddRangeAsync(relations);
        await arrangeContext.SaveChangesAsync();

        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(actDbContext, eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());
        UseCase.UpdateGenre updateGenre = new(
                                            new GenreRepository(actDbContext),
                                            unitOfWork,
                                            new CategoryRepository(actDbContext));
        UseCase.UpdateGenreInput input = new(targetGenre.Id,
            _fixture.GetValidGenreName(),
            !targetGenre.IsActive,
            newRelatedCategories.Select(x => x.Id).ToList());
        GenreModelOutput output = await updateGenre.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive!.Value);
        output.Categories.Should().HaveCount(newRelatedCategories.Count);
        List<Guid>? relatedCategoriesIdsOutput = output.Categories.Select(x => x.Id).ToList();
        relatedCategoriesIdsOutput.Should().BeEquivalentTo(input.CategoriesIds);
        //Assert in DB
        CodeflixCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
        DomainEntity.Genre? genreDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreDb.Should().NotBeNull();
        genreDb!.Id.Should().Be(targetGenre.Id);
        genreDb.Name.Should().Be(input.Name);
        genreDb.IsActive.Should().Be(input.IsActive!.Value);
        List<Guid>? relationsDbCategoriesIds = await assertDbContext.GenresCategories.AsNoTracking()
            .Where(relation => relation.GenreId == input.Id)
            .Select(relation => relation.CategoryId)
            .ToListAsync();
        relatedCategoriesIdsOutput.Should().BeEquivalentTo(relationsDbCategoriesIds);
    }

    [Fact(DisplayName = nameof(UpdateGenreThrowsWhenCategoryDoesntExists))]
    [Trait("Integration/Application", "Use Cases - UpdateGenre")]
    public async Task UpdateGenreThrowsWhenCategoryDoesntExists()
    {
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoryList(10);
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(10);
        CodeflixCatalogDbContext arrangeContext = _fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        List<DomainEntity.Category> relatedCategories = exampleCategories.GetRange(0, 5);
        List<DomainEntity.Category> newRelatedCategories = exampleCategories.GetRange(5, 3);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        List<GenresCategories> relations = targetGenre.Categories
            .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id))
            .ToList();
        await arrangeContext.AddRangeAsync(exampleGenres);
        await arrangeContext.AddRangeAsync(exampleCategories);
        await arrangeContext.AddRangeAsync(relations);
        await arrangeContext.SaveChangesAsync();

        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(actDbContext, eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());
        UseCase.UpdateGenre updateGenre = new(
                                            new GenreRepository(actDbContext),
                                            unitOfWork,
                                            new CategoryRepository(actDbContext));
        List<Guid> categoryIdsToRelate = newRelatedCategories.Select(category => category.Id).ToList();
        Guid invalidCatetoryId = Guid.NewGuid();
        categoryIdsToRelate.Add(invalidCatetoryId);
        UseCase.UpdateGenreInput input = new(targetGenre.Id,
            _fixture.GetValidGenreName(),
            !targetGenre.IsActive,
            categoryIdsToRelate);
        Func<Task<GenreModelOutput>> action = async () =>
            await updateGenre.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {invalidCatetoryId}.");
    }

    [Fact(DisplayName = nameof(UpdateGenreThrowsWhenNotFound))]
    [Trait("Integration/Application", "Use Cases - UpdateGenre")]
    public async Task UpdateGenreThrowsWhenNotFound()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(10);
        CodeflixCatalogDbContext arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(exampleGenres);
        await arrangeContext.SaveChangesAsync();
        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(actDbContext, eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());
        UseCase.UpdateGenre updateGenre = new(
                                            new GenreRepository(actDbContext),
                                            unitOfWork,
                                            new CategoryRepository(actDbContext));
        Guid invalidGuid = Guid.NewGuid();
        UseCase.UpdateGenreInput input = new(invalidGuid,
            _fixture.GetValidGenreName(),
            true);
        Func<Task<GenreModelOutput>> action = async () => 
            await updateGenre.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{invalidGuid}' not found.");
    }

    [Fact(DisplayName = nameof(UpdateGenreWithoutNewCategoriesRelations))]
    [Trait("Integration/Application", "Use Cases - UpdateGenre")]
    public async Task UpdateGenreWithoutNewCategoriesRelations()
    {
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoryList(10);
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(10, false);
        CodeflixCatalogDbContext arrangeContext = _fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        List<DomainEntity.Category> relatedCategories = exampleCategories.GetRange(0, 5);
        
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        List<GenresCategories> relations = targetGenre.Categories
            .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id))
            .ToList();
        await arrangeContext.AddRangeAsync(exampleGenres);
        await arrangeContext.AddRangeAsync(exampleCategories);
        await arrangeContext.AddRangeAsync(relations);
        await arrangeContext.SaveChangesAsync();

        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(actDbContext, eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());
        UseCase.UpdateGenre updateGenre = new(
                                            new GenreRepository(actDbContext),
                                            unitOfWork,
                                            new CategoryRepository(actDbContext));
        UseCase.UpdateGenreInput input = new(targetGenre.Id,
            _fixture.GetValidGenreName(),
            !targetGenre.IsActive);
        GenreModelOutput output = await updateGenre.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive!.Value);
        output.Categories.Should().HaveCount(relatedCategories.Count);
        List<Guid> expectedCategoryIds = relatedCategories.Select(c => c.Id).ToList();
        List<Guid>? relatedCategoriesIdsOutput = output.Categories.Select(x => x.Id).ToList();
        relatedCategoriesIdsOutput.Should().BeEquivalentTo(expectedCategoryIds);
        //Assert in DB
        CodeflixCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
        DomainEntity.Genre? genreDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreDb.Should().NotBeNull();
        genreDb!.Id.Should().Be(targetGenre.Id);
        genreDb.Name.Should().Be(input.Name);
        genreDb.IsActive.Should().Be(input.IsActive!.Value);
        List<Guid>? relationsDbCategoriesIds = await assertDbContext.GenresCategories.AsNoTracking()
            .Where(relation => relation.GenreId == input.Id)
            .Select(relation => relation.CategoryId)
            .ToListAsync();
        relatedCategoriesIdsOutput.Should().BeEquivalentTo(expectedCategoryIds);
    }

    [Fact(DisplayName = nameof(UpdateGenreWithoutEmptyCategoryIdsInRelations))]
    [Trait("Integration/Application", "Use Cases - UpdateGenre")]
    public async Task UpdateGenreWithoutEmptyCategoryIdsInRelations()
    {
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoryList(10);
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(10, false);
        CodeflixCatalogDbContext arrangeContext = _fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        List<DomainEntity.Category> relatedCategories = exampleCategories.GetRange(0, 5);

        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        List<GenresCategories> relations = targetGenre.Categories
            .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id))
            .ToList();
        await arrangeContext.AddRangeAsync(exampleGenres);
        await arrangeContext.AddRangeAsync(exampleCategories);
        await arrangeContext.AddRangeAsync(relations);
        await arrangeContext.SaveChangesAsync();

        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(actDbContext, eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());
        UseCase.UpdateGenre updateGenre = new(
                                            new GenreRepository(actDbContext),
                                            unitOfWork,
                                            new CategoryRepository(actDbContext));
        UseCase.UpdateGenreInput input = new(targetGenre.Id,
            _fixture.GetValidGenreName(),
            !targetGenre.IsActive, new List<Guid>());
        GenreModelOutput output = await updateGenre.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive!.Value);
        output.Categories.Should().HaveCount(0);
        
        List<Guid>? relatedCategoriesIdsOutput = output.Categories.Select(x => x.Id).ToList();
        relatedCategoriesIdsOutput.Should().BeEquivalentTo(new List<Guid>());
        //Assert in DB
        CodeflixCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
        DomainEntity.Genre? genreDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreDb.Should().NotBeNull();
        genreDb!.Id.Should().Be(targetGenre.Id);
        genreDb.Name.Should().Be(input.Name);
        genreDb.IsActive.Should().Be(input.IsActive!.Value);
        List<Guid>? relationsDbCategoriesIds = await assertDbContext.GenresCategories.AsNoTracking()
            .Where(relation => relation.GenreId == input.Id)
            .Select(relation => relation.CategoryId)
            .ToListAsync();
        relatedCategoriesIdsOutput.Should().BeEquivalentTo(new List<Guid>());
    }
}

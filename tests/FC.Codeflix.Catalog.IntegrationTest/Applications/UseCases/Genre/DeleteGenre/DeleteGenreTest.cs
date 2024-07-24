using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;
namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Genre.DeleteGenre;

[Collection(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTest
{
    private readonly DeleteGenreTestFixture _fixture;

    public DeleteGenreTest(DeleteGenreTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("Integration/Application", "Use Cases - DeleteGenre")]
    public async Task DeleteGenre()
    {
        var genresExampleList = _fixture.GetExampleGenresList(10);
        var targetGenre = genresExampleList[5];
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var useCase = new UseCase.DeleteGenre(
            new GenreRepository(actDbContext), new UnitOfWork(actDbContext)
            );
        var input = new UseCase.DeleteGenreInput(targetGenre.Id);

        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = _fixture.CreateDbContext(true);
        var genreDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreDb.Should().BeNull();
    }

    [Fact(DisplayName = nameof(DeleteGenreWithRelations))]
    [Trait("Integration/Application", "Use Cases - DeleteGenre")]
    public async Task DeleteGenreWithRelations()
    {
        var genresExampleList = _fixture.GetExampleGenresList(10);
        var targetGenre = genresExampleList[5];
        var exampleCategories = _fixture.GetExampleCategoryList(5);
        var relations = exampleCategories.Select(
            r => new GenresCategories(r.Id, targetGenre.Id)
        );
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.Categories.AddRangeAsync(exampleCategories);
        await dbArrangeContext.GenresCategories.AddRangeAsync(relations);
        await dbArrangeContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var useCase = new UseCase.DeleteGenre(
            new GenreRepository(actDbContext), new UnitOfWork(actDbContext)
            );
        var input = new UseCase.DeleteGenreInput(targetGenre.Id);

        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = _fixture.CreateDbContext(true);
        var genreDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreDb.Should().BeNull();
        var relationsDb = await actDbContext.GenresCategories
            .AsNoTracking()
            .Where(r => r.GenreId == targetGenre.Id)
            .ToListAsync();
        relationsDb.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ThrowsWhenNotFound))]
    [Trait("Integration/Application", "Use Cases - DeleteGenre")]
    public async Task ThrowsWhenNotFound()
    {
        var genresExampleList = _fixture.GetExampleGenresList(10);
        var randomGuid = Guid.NewGuid();
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var useCase = new UseCase.DeleteGenre(
            new GenreRepository(actDbContext), new UnitOfWork(actDbContext)
            );
        var input = new UseCase.DeleteGenreInput(randomGuid);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{randomGuid}' not found.");

    }
}

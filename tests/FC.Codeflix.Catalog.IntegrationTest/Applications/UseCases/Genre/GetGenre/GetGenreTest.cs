using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

using FluentAssertions;

using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre;
namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Genre.GetGenre;

[Collection(nameof(GetGenreTestFixture))]
public class GetGenreTest
{
    private readonly GetGenreTestFixture _fixture;

    public GetGenreTest(GetGenreTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("Integration/Application", "Use Cases - GetGenre")]
    public async Task GetGenre()
    {
        var genresExampleList = _fixture.GetExampleGenresList(10);
        var expectedGenre = genresExampleList[5];
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(dbArrangeContext);
        var useCase = new UseCase.GetGenre(genreRepository);
        var input = new UseCase.GetGenreInput(expectedGenre.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(expectedGenre.Id);
        output.Name.Should().Be(expectedGenre.Name);
        output.IsActive.Should().Be(expectedGenre.IsActive);
        output.CreatedAt.Should().Be(expectedGenre.CreatedAt);
    }

    [Fact(DisplayName = nameof(GetGenreThrowsWhenNotFound))]
    [Trait("Integration/Application", "Use Cases - GetGenre")]
    public async Task GetGenreThrowsWhenNotFound()
    {
        var genresExampleList = _fixture.GetExampleGenresList(10);
        var ramdonGuid = Guid.NewGuid();
        var expectedGenre = genresExampleList[5];
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(dbArrangeContext);
        var useCase = new UseCase.GetGenre(genreRepository);
        var input = new UseCase.GetGenreInput(ramdonGuid);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{ramdonGuid}' not found.");
    }

    [Fact(DisplayName = nameof(GetGenreWithCategoryRelations))]
    [Trait("Integration/Application", "Use Cases - GetGenre")]
    public async Task GetGenreWithCategoryRelations()
    {
        var genresExampleList = _fixture.GetExampleGenresList(10);
        var categoriesExampleList = _fixture.GetExampleCategoryList(5);
        
        var expectedGenre = genresExampleList[5];
        categoriesExampleList.ForEach(category => 
            expectedGenre.AddCategory(category.Id)
            );
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.AddRangeAsync(categoriesExampleList);
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.GenresCategories.AddRangeAsync(
            expectedGenre.Categories.Select(categoryId => 
                new GenresCategories(categoryId, expectedGenre.Id)
                ).ToList()
        );
        await dbArrangeContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(dbArrangeContext);
        var useCase = new UseCase.GetGenre(genreRepository);
        var input = new UseCase.GetGenreInput(expectedGenre.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(expectedGenre.Id);
        output.Name.Should().Be(expectedGenre.Name);
        output.IsActive.Should().Be(expectedGenre.IsActive);
        output.CreatedAt.Should().Be(expectedGenre.CreatedAt);
        output.Categories.Should().HaveCount(expectedGenre.Categories.Count);
        output.Categories.ToList().ForEach(
            relationModel => {
                expectedGenre.Categories.Should().Contain(relationModel.Id);
                relationModel.Name.Should().BeNull();
            }
        );

    }
}

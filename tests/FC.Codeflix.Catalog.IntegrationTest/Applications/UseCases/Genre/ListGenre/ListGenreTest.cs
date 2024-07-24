using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

using FluentAssertions;

using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenres;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Genre.ListGenre;

[Collection(nameof(ListGenreTestFixture))]
public class ListGenreTest
{
    private readonly ListGenreTestFixture _fixture;

    public ListGenreTest(ListGenreTestFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = nameof(ListGenres))]
    [Trait("Integration/Application", "Use Cases - ListGenre")]
    public async Task ListGenres()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(10, false);
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.Genres.AddRangeAsync(exampleGenres);
        await arrangeDbContext.SaveChangesAsync();
        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        UseCase.ListGenres useCase = new(new GenreRepository(actDbContext), new CategoryRepository(actDbContext));
        UseCase.ListGenresInput input = new(1, 20);

        UseCase.ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleGenres.Count);
        output.Items.Should().HaveCount(exampleGenres.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            DomainEntity.Genre? exampleItem =
                exampleGenres.Find(example => example.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);

        });
    }

    [Fact(DisplayName = nameof(ListGenresReturnsEmptyWhenPersistenceIsEmpty))]
    [Trait("Integration/Application", "Use Cases - ListGenre")]
    public async Task ListGenresReturnsEmptyWhenPersistenceIsEmpty()
    {
        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext();
        UseCase.ListGenres useCase = new(new GenreRepository(actDbContext), new CategoryRepository(actDbContext));
        UseCase.ListGenresInput input = new(1, 20);
        UseCase.ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);

    }

    [Fact(DisplayName = nameof(ListGenresVerifyRelations))]
    [Trait("Integration/Application", "Use Cases - ListGenre")]
    public async Task ListGenresVerifyRelations()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(10, false);
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoryList(10);
        Random random = new();
        exampleGenres.ForEach(genre =>
        {
            int relationsCount = random.Next(0, 3);
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
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.Genres.AddRangeAsync(exampleGenres);
        await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(genresCategories);
        await arrangeDbContext.SaveChangesAsync();
        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        UseCase.ListGenres useCase = new(new GenreRepository(actDbContext), new CategoryRepository(actDbContext));
        UseCase.ListGenresInput input = new(1, 20);

        UseCase.ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleGenres.Count);
        output.Items.Should().HaveCount(exampleGenres.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            DomainEntity.Genre? exampleItem =
                exampleGenres.Find(example => example.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            List<Guid> outputItemsCategoryIds = outputItem.Categories.Select(c => c.Id).ToList();
            outputItemsCategoryIds.Should().BeEquivalentTo(exampleItem.Categories);
            outputItem.Categories.ToList().ForEach((outputCategory) => {
                DomainEntity.Category? exampleCategory = exampleCategories.Find(x => x.Id == outputCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputCategory.Name.Should().Be(exampleCategory!.Name);
            });
        });
    }

    [Theory(DisplayName = nameof(ListGenresPagginated))]
    [Trait("Integration/Application", "Use Cases - ListGenre")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListGenresPagginated(
        int quantityToGenerate, 
        int page, 
        int perPage, 
        int expectedQuantityItems)
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(quantityToGenerate, false);
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoryList(10);
        Random random = new();
        exampleGenres.ForEach(genre =>
        {
            int relationsCount = random.Next(0, 3);
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
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.Genres.AddRangeAsync(exampleGenres);
        await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(genresCategories);
        await arrangeDbContext.SaveChangesAsync();
        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        UseCase.ListGenres useCase = new(new GenreRepository(actDbContext), new CategoryRepository(actDbContext));
        UseCase.ListGenresInput input = new(page, perPage);

        UseCase.ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleGenres.Count);
        output.Items.Should().HaveCount(expectedQuantityItems);
        output.Items.ToList().ForEach(outputItem =>
        {
            DomainEntity.Genre? exampleItem =
                exampleGenres.Find(example => example.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            List<Guid> outputItemsCategoryIds = outputItem.Categories.Select(c => c.Id).ToList();
            outputItemsCategoryIds.Should().BeEquivalentTo(exampleItem.Categories);
            outputItem.Categories.ToList().ForEach((outputCategory) => {
                DomainEntity.Category? exampleCategory = exampleCategories.Find(x => x.Id == outputCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputCategory.Name.Should().Be(exampleCategory!.Name);
            });
        });
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Application", "Use Cases - ListGenre")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(
        string search, int page, int perPage, int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems)
    {
        var exampleGenresList = _fixture.GetExampleGenresListByNames(new List<string>()
        {
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Space",
            "Sci-fi Robots",
            "Sci-fi Future",
        });
        
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoryList(10);
        Random random = new();
        exampleGenresList.ForEach(genre =>
        {
            int relationsCount = random.Next(0, 3);
            for (int i = 0; i < relationsCount; i++)
            {
                int selectedCategoryIndex = random.Next(exampleCategories.Count - 1);
                DomainEntity.Category selected = exampleCategories[selectedCategoryIndex];
                if (!genre.Categories.Contains(selected.Id))
                    genre.AddCategory(selected.Id);
            }
        });
        List<GenresCategories> genresCategories = new();
        exampleGenresList.ForEach(genre =>
            genre.Categories.ToList().ForEach(
                categoryId => genresCategories.Add(new GenresCategories(categoryId, genre.Id))
            )
        );
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.Genres.AddRangeAsync(exampleGenresList);
        await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(genresCategories);
        await arrangeDbContext.SaveChangesAsync();
        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        UseCase.ListGenres useCase = new(new GenreRepository(actDbContext), new CategoryRepository(actDbContext));
        UseCase.ListGenresInput input = new(page, perPage, search);

        UseCase.ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(expectedQuantityTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        output.Items.ToList().ForEach(outputItem =>
        {
            DomainEntity.Genre? exampleItem =
                exampleGenresList.Find(example => example.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            List<Guid> outputItemsCategoryIds = outputItem.Categories.Select(c => c.Id).ToList();
            outputItemsCategoryIds.Should().BeEquivalentTo(exampleItem.Categories);
            outputItem.Categories.ToList().ForEach((outputCategory) => {
                DomainEntity.Category? exampleCategory = exampleCategories.Find(x => x.Id == outputCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputCategory.Name.Should().Be(exampleCategory!.Name);
            });
        });
    }

    [Theory(DisplayName = nameof(Ordered))]
    [Trait("Integration/Application", "Use Cases - ListGenre")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "desc")]
    public async Task Ordered(string orderBy, string order)
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleGenresList(10, false);
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoryList(10);
        Random random = new();
        exampleGenres.ForEach(genre =>
        {
            int relationsCount = random.Next(0, 3);
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
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.Genres.AddRangeAsync(exampleGenres);
        await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(genresCategories);
        await arrangeDbContext.SaveChangesAsync();
        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        UseCase.ListGenres useCase = new(new GenreRepository(actDbContext), new CategoryRepository(actDbContext));
        var orderEnum = order == "asc"? SearchOrder.Asc : SearchOrder.Desc;
        UseCase.ListGenresInput input = new(1, 20, sort: orderBy, dir: orderEnum);

        UseCase.ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        var expectedOrderedList = _fixture.CloneGenresListOrdered(
          exampleGenres,
          orderBy,
          orderEnum);
        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleGenres.Count);
        output.Items.Should().HaveCount(exampleGenres.Count);
        for (int i = 0; i < expectedOrderedList.Count; i++)
        {
            var expectedItem = expectedOrderedList[i];
            var outputItem = output.Items[i];

            expectedItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem!.Id.Should().Be(expectedItem!.Id);
            outputItem!.Name.Should().Be(expectedItem!.Name);
            outputItem.IsActive.Should().Be(expectedItem!.IsActive);
            outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
        output.Items.ToList().ForEach(outputItem =>
        {
            DomainEntity.Genre? exampleItem =
                exampleGenres.Find(example => example.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            List<Guid> outputItemsCategoryIds = outputItem.Categories.Select(c => c.Id).ToList();
            outputItemsCategoryIds.Should().BeEquivalentTo(exampleItem.Categories);
            outputItem.Categories.ToList().ForEach((outputCategory) => {
                DomainEntity.Category? exampleCategory = exampleCategories.Find(x => x.Id == outputCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputCategory.Name.Should().Be(exampleCategory!.Name);
            });
        });
    }
}

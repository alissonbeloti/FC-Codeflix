using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace FC.Codeflix.Catalog.IntegrationTest.Infra.Data.EF.Repositories.GenreRepository;

[Collection(nameof(GenreRepositoryTestFixture))]
public class GenreRepositoryTest
{
    private readonly GenreRepositoryTestFixture _fixture;

    public GenreRepositoryTest(GenreRepositoryTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "Repositories - GenreRepository")]
    public async Task Insert()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoryList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var genreRepository = new Repository.GenreRepository(dbContext);

        await genreRepository.Insert(exampleGenre, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);

        dbGenre.Should().NotBeNull();
        dbGenre!.Id.Should().Be(exampleGenre.Id);
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre!.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genreCategoriesRelations = await assertsDbContext.GenresCategories
            .Where(r => r.GenreId == exampleGenre.Id)
            .ToListAsync();
        genreCategoriesRelations.Should().HaveCount(categoriesListExample.Count);
        genreCategoriesRelations.ForEach(r =>
        {
            var expected = categoriesListExample.FirstOrDefault(x => x.Id == r.CategoryId);
            expected.Should().NotBeNull();
        });

    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "Repositories - GenreRepository")]
    public async Task Get()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoryList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            await dbContext.AddAsync(new GenresCategories(categoryId, exampleGenre.Id));
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var genreRepository = new Repository.GenreRepository(_fixture.CreateDbContext(true));

        var genreOutput = await genreRepository.Get(exampleGenre.Id, CancellationToken.None);

        genreOutput.Should().NotBeNull();
        genreOutput!.Id.Should().Be(exampleGenre.Id);
        genreOutput!.Name.Should().Be(exampleGenre.Name);
        genreOutput.IsActive.Should().Be(exampleGenre!.IsActive);
        genreOutput.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        genreOutput.Categories.Should().HaveCount(genreOutput.Categories.Count);
        foreach (var categoryId in genreOutput.Categories)
        {
            var expected = categoriesListExample.FirstOrDefault(x => x.Id == categoryId);
            expected.Should().NotBeNull();
        }
    }

    [Fact(DisplayName = nameof(GetThrowWhenNotFound))]
    [Trait("Integration/Infra.Data", "Repositories - GenreRepository")]
    public async Task GetThrowWhenNotFound()
    {
        var exampleNotFoundGuid = Guid.NewGuid();
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoryList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            await dbContext.AddAsync(new GenresCategories(categoryId, exampleGenre.Id));
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var genreRepository = new Repository.GenreRepository(_fixture.CreateDbContext(true));

        var action = async () => await genreRepository.Get(exampleNotFoundGuid, CancellationToken.None);

        action.Should().NotBeNull();
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{exampleNotFoundGuid}' not found.");
        
    }

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "Repositories - GenreRepository")]
    public async Task Delete()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoryList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            await dbContext.AddAsync(new GenresCategories(categoryId, exampleGenre.Id));
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repositoryDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(repositoryDbContext);

        await genreRepository.Delete(exampleGenre, CancellationToken.None);
        await repositoryDbContext.SaveChangesAsync();

        var assertDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertDbContext.Genres.AsNoTracking().FirstOrDefaultAsync(
            g => g.Id == exampleGenre.Id);
        dbGenre.Should().BeNull();
        var categoriesIdsList = await assertDbContext.GenresCategories.AsNoTracking()
            .Where(r => r.GenreId == exampleGenre.Id)
            .Select(_ => _.CategoryId)
            .ToListAsync();
        categoriesIdsList.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "Repositories - GenreRepository")]
    public async Task Update()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoryList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            await dbContext.AddAsync(new GenresCategories(categoryId, exampleGenre.Id));
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(actDbContext);
        exampleGenre.Update(_fixture.GetValidGenreName());
        if (exampleGenre.IsActive) 
            exampleGenre.Deactivate(); 
        else 
            exampleGenre.Activate();
        await genreRepository.Update(exampleGenre, CancellationToken.None);
        await actDbContext.SaveChangesAsync();

        var assertContext = _fixture.CreateDbContext(true);
        var genreFromDb = await assertContext.Genres.FindAsync(exampleGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(exampleGenre.Id);
        genreFromDb.Name.Should().Be(exampleGenre.Name);
        genreFromDb.IsActive.Should().Be(exampleGenre!.IsActive);
        genreFromDb.CreatedAt.Should().Be(exampleGenre.CreatedAt);

        var genreCategoriesRelations = await assertContext.GenresCategories
            .Where(r => r.GenreId == exampleGenre.Id)
            .ToListAsync();
        genreCategoriesRelations.Should().HaveCount(categoriesListExample.Count);
        genreCategoriesRelations.ForEach(r =>
        {
            var expected = categoriesListExample.FirstOrDefault(x => x.Id == r.CategoryId);
            expected.Should().NotBeNull();
        });
    }

    [Fact(DisplayName = nameof(UpdateRemovingRelations))]
    [Trait("Integration/Infra.Data", "Repositories - GenreRepository")]
    public async Task UpdateRemovingRelations()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoryList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            await dbContext.AddAsync(new GenresCategories(categoryId, exampleGenre.Id));
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(actDbContext);
        exampleGenre.Update(_fixture.GetValidGenreName());
        if (exampleGenre.IsActive)
            exampleGenre.Deactivate();
        else
            exampleGenre.Activate();
        exampleGenre.RemoveAllCategories();
        await genreRepository.Update(exampleGenre, CancellationToken.None);
        await actDbContext.SaveChangesAsync();

        var assertContext = _fixture.CreateDbContext(true);
        var genreFromDb = await assertContext.Genres.FindAsync(exampleGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(exampleGenre.Id);
        genreFromDb.Name.Should().Be(exampleGenre.Name);
        genreFromDb.IsActive.Should().Be(exampleGenre!.IsActive);
        genreFromDb.CreatedAt.Should().Be(exampleGenre.CreatedAt);

        var genreCategoriesRelations = await assertContext.GenresCategories
            .Where(r => r.GenreId == exampleGenre.Id)
            .ToListAsync();
        genreCategoriesRelations.Should().HaveCount(0);
        
    }

    [Fact(DisplayName = nameof(UpdateReplacingRelations))]
    [Trait("Integration/Infra.Data", "Repositories - GenreRepository")]
    public async Task UpdateReplacingRelations()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoryList(3);
        var updateCategoriesListExample = _fixture.GetExampleCategoryList(5);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Categories.AddRangeAsync(updateCategoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            await dbContext.AddAsync(new GenresCategories(categoryId, exampleGenre.Id));
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(actDbContext);
        exampleGenre.Update(_fixture.GetValidGenreName());
        if (exampleGenre.IsActive)
            exampleGenre.Deactivate();
        else
            exampleGenre.Activate();
        exampleGenre.RemoveAllCategories();
        updateCategoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await genreRepository.Update(exampleGenre, CancellationToken.None);
        await actDbContext.SaveChangesAsync();

        var assertContext = _fixture.CreateDbContext(true);
        var genreFromDb = await assertContext.Genres.FindAsync(exampleGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(exampleGenre.Id);
        genreFromDb.Name.Should().Be(exampleGenre.Name);
        genreFromDb.IsActive.Should().Be(exampleGenre!.IsActive);
        genreFromDb.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genreCategoriesRelations = await assertContext.GenresCategories
            .Where(r => r.GenreId == exampleGenre.Id)
            .ToListAsync();
        genreCategoriesRelations.Should().HaveCount(updateCategoriesListExample.Count);
        genreCategoriesRelations.ForEach(r =>
        {
            var expected = updateCategoriesListExample.FirstOrDefault(x => x.Id == r.CategoryId);
            expected.Should().NotBeNull();
        });
    }
    
    [Fact(DisplayName = nameof(SearchReturnsItemsAndTotal))]
    [Trait("Integration/Infra.Data", "Repositories - GenreRepository")]
    public async Task SearchReturnsItemsAndTotal()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresList();
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(actDbContext);

        var searchResult = await genreRepository.SearchAsync(searchInput, CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.Total.Should().Be(exampleGenresList.Count);
        
        searchResult.Items.Should().HaveCount(exampleGenresList.Count);
        foreach (var resultItem in searchResult.Items)
        {
            var examapleGenre = exampleGenresList.Find(x => x.Id == resultItem.Id);
            examapleGenre.Should().NotBeNull();
            examapleGenre!.Name.Should().Be(resultItem.Name);
            examapleGenre!.IsActive.Should().Be(resultItem.IsActive);
            examapleGenre!.CreatedAt.Should().Be(resultItem.CreatedAt);
        }
    }

    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenPersistenceIsEmpty))]
    [Trait("Integration/Infra.Data", "Repositories - GenreRepository")]
    public async Task SearchReturnsEmptyWhenPersistenceIsEmpty()
    {
        
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);
        var actDbContext = _fixture.CreateDbContext();
        var genreRepository = new Repository.GenreRepository(actDbContext);

        var searchResult = await genreRepository.SearchAsync(searchInput, CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.Total.Should().Be(0);
        searchResult.Items.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(SearchReturnsRelations))]
    [Trait("Integration/Infra.Data", "Repositories - GenreRepository")]
    public async Task SearchReturnsRelations()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresList(10, false);
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        var random = new Random();
        exampleGenresList.ForEach(exampleGenre =>
        {
            var categoriesListToRelation = _fixture.GetExampleCategoryList(random.Next(0, 4));
            if (categoriesListToRelation.Count > 0)
            {
                categoriesListToRelation.ForEach(category =>
                {
                    exampleGenre.AddCategory(category.Id);
                });
                dbContext.Categories.AddRange(categoriesListToRelation);
                var relationsToAdd = categoriesListToRelation
                    .Select(x => new GenresCategories(x.Id, exampleGenre.Id)).ToList();
                dbContext.GenresCategories.AddRange(relationsToAdd);
            }
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(actDbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        var searchResult = await genreRepository.SearchAsync(searchInput, CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.Total.Should().Be(exampleGenresList.Count);

        searchResult.Items.Should().HaveCount(exampleGenresList.Count);
        foreach (var resultItem in searchResult.Items)
        {
            var examapleGenre = exampleGenresList.Find(x => x.Id == resultItem.Id);
            examapleGenre.Should().NotBeNull();
            examapleGenre!.Name.Should().Be(resultItem.Name);
            examapleGenre!.IsActive.Should().Be(resultItem.IsActive);
            examapleGenre!.CreatedAt.Should().Be(resultItem.CreatedAt);
            examapleGenre.Categories.Should().BeEquivalentTo(resultItem.Categories);
            foreach (var categoryId in resultItem.Categories)
            {
                examapleGenre.Categories.Contains(categoryId).Should().BeTrue();
            }
        }
    }

    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [Trait("Integration/Infra.Data", "Repositories - GenreRepository")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchReturnsPaginated(int quantityToGenerate, int page, int perPage, int expectedQuantityItems)
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresList(quantityToGenerate, false);
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        var random = new Random();
        exampleGenresList.ForEach(exampleGenre =>
        {
            var categoriesListToRelation = _fixture.GetExampleCategoryList(random.Next(0, 4));
            if (categoriesListToRelation.Count > 0)
            {
                categoriesListToRelation.ForEach(category =>
                {
                    exampleGenre.AddCategory(category.Id);
                });
                dbContext.Categories.AddRange(categoriesListToRelation);
                var relationsToAdd = categoriesListToRelation
                    .Select(x => new GenresCategories(x.Id, exampleGenre.Id)).ToList();
                dbContext.GenresCategories.AddRange(relationsToAdd);
            }
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(actDbContext);
        var searchInput = new SearchInput(page, perPage, "", "", SearchOrder.Asc);

        var searchResult = await genreRepository.SearchAsync(searchInput, CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.Total.Should().Be(exampleGenresList.Count);

        searchResult.Items.Should().HaveCount(expectedQuantityItems);

        foreach (var resultItem in searchResult.Items)
        {
            var examapleGenre = exampleGenresList.Find(x => x.Id == resultItem.Id);
            examapleGenre.Should().NotBeNull();
            examapleGenre!.Name.Should().Be(resultItem.Name);
            examapleGenre!.IsActive.Should().Be(resultItem.IsActive);
            examapleGenre!.CreatedAt.Should().Be(resultItem.CreatedAt);
            examapleGenre.Categories.Should().BeEquivalentTo(resultItem.Categories);
            foreach (var categoryId in resultItem.Categories)
            {
                examapleGenre.Categories.Contains(categoryId).Should().BeTrue();
            }
        }
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Infra.Data", "Repositories - GenreRepository")]
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
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
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
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        var random = new Random();
        exampleGenresList.ForEach(exampleGenre =>
        {
            var categoriesListToRelation = _fixture.GetExampleCategoryList(random.Next(0, 4));
            if (categoriesListToRelation.Count > 0)
            {
                categoriesListToRelation.ForEach(category =>
                {
                    exampleGenre.AddCategory(category.Id);
                });
                dbContext.Categories.AddRange(categoriesListToRelation);
                var relationsToAdd = categoriesListToRelation
                    .Select(x => new GenresCategories(x.Id, exampleGenre.Id)).ToList();
                dbContext.GenresCategories.AddRange(relationsToAdd);
            }
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(actDbContext);
        var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.Asc);

        var searchResult = await genreRepository.SearchAsync(searchInput, CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.Total.Should().Be(expectedQuantityTotalItems);

        searchResult.Items.Should().HaveCount(expectedQuantityItemsReturned);

        foreach (var resultItem in searchResult.Items)
        {
            var examapleGenre = exampleGenresList.Find(x => x.Id == resultItem.Id);
            examapleGenre.Should().NotBeNull();
            examapleGenre!.Name.Should().Be(resultItem.Name);
            examapleGenre!.IsActive.Should().Be(resultItem.IsActive);
            examapleGenre!.CreatedAt.Should().Be(resultItem.CreatedAt);
            examapleGenre.Categories.Should().BeEquivalentTo(resultItem.Categories);
            foreach (var categoryId in resultItem.Categories)
            {
                examapleGenre.Categories.Contains(categoryId).Should().BeTrue();
            }
        }
    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Infra.Data", "Repositories - GenreRepository")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "desc")]
    public async Task SearchOrdered(string orderBy, string order)
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenreList = _fixture.GetExampleGenresList(10);
        await dbContext.AddRangeAsync(exampleGenreList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var genreRepository = new Repository.GenreRepository(dbContext);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;

        var serachInput = new SearchInput(1, 20, "", orderBy, searchOrder);

        var output = await genreRepository.SearchAsync(serachInput, CancellationToken.None);

        var expectedOrderedList = _fixture.CloneGenresListOrdered(
            exampleGenreList,
            orderBy,
            searchOrder);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(serachInput.Page);
        output.PerPage.Should().Be(serachInput.PerPage);
        output.Total.Should().Be(exampleGenreList.Count);
        output.Items.Should().HaveCount(exampleGenreList.Count);
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
    }

    [Fact(DisplayName = nameof(GetIdsListByIds))]
    [Trait("Integration/Infra.Data", "Repositories - GenreRepository")]
    public async Task GetIdsListByIds()
    {
        var arrangeDbContext = _fixture.CreateDbContext();
        var exampleGenreList = _fixture.GetExampleGenresList(10);
        await arrangeDbContext.AddRangeAsync(exampleGenreList);
        await arrangeDbContext.SaveChangesAsync(CancellationToken.None);
        var actDbContext = _fixture.CreateDbContext(true);
        var repository = new Repository.GenreRepository(arrangeDbContext);
        var idsToGet = new List<Guid>()
        {
            exampleGenreList[0].Id,
            exampleGenreList[1].Id
        };

        var result = await repository.GetIdsListByIds(idsToGet, CancellationToken.None);

        result.ToList().Should().HaveCount(idsToGet.Count);
        result.ToList().Should().BeEquivalentTo(idsToGet);
    }

    [Fact(DisplayName = nameof(GetIdsListByIdsWhenOnlyThreeIdsMatch))]
    [Trait("Integration/Infra.Data", "Repositories - GenreRepository")]
    public async Task GetIdsListByIdsWhenOnlyThreeIdsMatch()
    {
        var arrangeDbContext = _fixture.CreateDbContext();
        var exampleGenreList = _fixture.GetExampleGenresList(10);
        await arrangeDbContext.AddRangeAsync(exampleGenreList);
        await arrangeDbContext.SaveChangesAsync(CancellationToken.None);
        var actDbContext = _fixture.CreateDbContext(true);
        var repository = new Repository.GenreRepository(actDbContext);
        var idsToGet = new List<Guid>()
        {
            exampleGenreList[3].Id,
            exampleGenreList[4].Id,
            exampleGenreList[5].Id,
            Guid.NewGuid(),
            Guid.NewGuid(),
        };
        var idsExpectedToReturn = new List<Guid>()
        {
            exampleGenreList[3].Id,
            exampleGenreList[4].Id,
            exampleGenreList[5].Id,
        };

        var result = await repository.GetIdsListByIds(idsToGet, CancellationToken.None);

        result.ToList().Should().HaveCount(3);
        result.ToList().Should().NotBeEquivalentTo(idsToGet);
        result.ToList().Should().BeEquivalentTo(idsExpectedToReturn);
    }
    
    [Fact(DisplayName = nameof(GetListByIds))]
    [Trait("Integration/Infra.Data", "Repositories - GenreRepository")]
    public async Task GetListByIds()
    {
        var arrangeDbContext = _fixture.CreateDbContext();
        var exampleGenreList = _fixture.GetExampleGenresList(10);
        await arrangeDbContext.AddRangeAsync(exampleGenreList);
        await arrangeDbContext.SaveChangesAsync(CancellationToken.None);
        var actDbContext = _fixture.CreateDbContext(true);
        var repository = new Repository.GenreRepository(actDbContext);
        var idsToGet = new List<Guid>()
        {
            exampleGenreList[3].Id,
            exampleGenreList[4].Id,
            exampleGenreList[5].Id,
        };
       
        var result = await repository.GetListByIds(idsToGet, CancellationToken.None);

        result.Should().NotBeNull();
        result.ToList().Should().HaveCount(idsToGet.Count);
        idsToGet.ForEach(id =>
        {
            var example = exampleGenreList.FirstOrDefault(x => x.Id == id);
            var resultItem = result.FirstOrDefault(x => x.Id == id);
            resultItem.Should().NotBeNull();
            example.Should().NotBeNull();
            resultItem!.Name.Should().Be(example!.Name);
            resultItem.Id.Should().Be(example.Id);
            resultItem.IsActive.Should().Be(example.IsActive);
        });
        
    }

    [Fact(DisplayName = nameof(GetListByIds))]
    [Trait("Integration/Infra.Data", "Repositories - GenreRepository")]
    public async Task GetListByIdsWhenThreeMatch()
    {
        var arrangeDbContext = _fixture.CreateDbContext();
        var exampleGenreList = _fixture.GetExampleGenresList(10);
        await arrangeDbContext.AddRangeAsync(exampleGenreList);
        await arrangeDbContext.SaveChangesAsync(CancellationToken.None);
        var actDbContext = _fixture.CreateDbContext(true);
        var repository = new Repository.GenreRepository(actDbContext);
        var idsToGet = new List<Guid>()
        {
            exampleGenreList[3].Id,
            exampleGenreList[4].Id,
            exampleGenreList[5].Id,
            Guid.NewGuid(),
            Guid.NewGuid(),
        };
        var expectedIdsToReturn =  new List<Guid>()
        {
            exampleGenreList[3].Id,
            exampleGenreList[4].Id,
            exampleGenreList[5].Id,
        };

        var result = await repository.GetListByIds(idsToGet, CancellationToken.None);

        result.Should().NotBeNull();
        result.ToList().Should().HaveCount(expectedIdsToReturn.Count);
        expectedIdsToReturn.ForEach(id =>
        {
            var example = exampleGenreList.FirstOrDefault(x => x.Id == id);
            var resultItem = result.FirstOrDefault(x => x.Id == id);
            resultItem.Should().NotBeNull();
            example.Should().NotBeNull();
            resultItem!.Name.Should().Be(example!.Name);
            resultItem.Id.Should().Be(example.Id);
            resultItem.IsActive.Should().Be(example.IsActive);
        });

    }
}

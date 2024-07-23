using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;

using FluentAssertions;

using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;


namespace FC.Codeflix.Catalog.IntegrationTest.Infra.Data.EF.Repositories.CategoryRepository;
[Collection(nameof(CategoryRepositoryTestFixture))]
public class CategoryRepositoryTest
{
    private readonly CategoryRepositoryTestFixture fixture;

    public CategoryRepositoryTest(CategoryRepositoryTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "Repositories - CategoryRepository")]
    public async Task Insert()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleCategory = fixture.GetExampleCategory();
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        await categoryRepository.Insert(exampleCategory, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var dbCategory = await (fixture.CreateDbContext(true)).Categories.FindAsync(exampleCategory.Id);

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(exampleCategory.Name);
        dbCategory.Description.Should().Be(exampleCategory?.Description);
        dbCategory.IsActive.Should().Be(exampleCategory!.IsActive);
        dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "Repositories - CategoryRepository")]
    public async Task Get()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleCategory = fixture.GetExampleCategory();
        var exampleCategoryList = fixture.GetExampleCategoryList(15);
        exampleCategoryList.Add(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(fixture.CreateDbContext(true));

        var dbCategory = await categoryRepository.Get(exampleCategory.Id, CancellationToken.None);

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(exampleCategory.Name);
        dbCategory!.Id.Should().Be(exampleCategory.Id);
        dbCategory.Description.Should().Be(exampleCategory?.Description);
        dbCategory.IsActive.Should().Be(exampleCategory!.IsActive);
        dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(GetThrowIfNotFound))]
    [Trait("Integration/Infra.Data", "Repositories - CategoryRepository")]
    public async Task GetThrowIfNotFound()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleId = Guid.NewGuid();
        var exampleCategoryList = fixture.GetExampleCategoryList(15);

        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        var task = async () => await categoryRepository.Get(exampleId, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Category '{exampleId}' not found.");
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "Repositories - CategoryRepository")]
    public async Task Update()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleCategory = fixture.GetExampleCategory();
        var newCategoryValues = fixture.GetExampleCategory();
        var exampleCategoryList = fixture.GetExampleCategoryList(15);
        exampleCategoryList.Add(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        exampleCategory.Update(newCategoryValues.Name, newCategoryValues.Description);

        await categoryRepository.Update(exampleCategory, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var dbCategory = await (fixture.CreateDbContext(true)).Categories.FindAsync(exampleCategory.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(exampleCategory.Name);
        dbCategory!.Id.Should().Be(exampleCategory.Id);
        dbCategory.Description.Should().Be(exampleCategory?.Description);
        dbCategory.IsActive.Should().Be(exampleCategory!.IsActive);
        dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "Repositories - CategoryRepository")]
    public async Task Delete()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleCategory = fixture.GetExampleCategory();
        var exampleCategoryList = fixture.GetExampleCategoryList(15);
        exampleCategoryList.Add(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        await categoryRepository.Delete(exampleCategory, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var dbCategory = await (fixture.CreateDbContext(true)).Categories.FindAsync(exampleCategory.Id);
        dbCategory.Should().BeNull();

    }

    [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
    [Trait("Integration/Infra.Data", "Repositories - CategoryRepository")]
    public async Task SearchReturnsListAndTotal()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleCategoryList = fixture.GetExampleCategoryList(15);
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        var serachInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        var output = await categoryRepository.SearchAsync(serachInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(serachInput.Page);
        output.PerPage.Should().Be(serachInput.PerPage);
        output.Total.Should().Be(exampleCategoryList.Count);
        output.Items.Should().HaveCount(exampleCategoryList.Count);

        foreach (Category outputItem in output.Items)
        {
            var exampleItem = exampleCategoryList.Find(ecl => ecl.Id == outputItem.Id);

            exampleItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem?.Description);
            outputItem.IsActive.Should().Be(exampleItem!.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }

    }

    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenPersistenceIsEmpty))]
    [Trait("Integration/Infra.Data", "Repositories - CategoryRepository")]
    public async Task SearchReturnsEmptyWhenPersistenceIsEmpty()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        var serachInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        var output = await categoryRepository.SearchAsync(serachInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(serachInput.Page);
        output.PerPage.Should().Be(serachInput.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }

    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [Trait("Integration/Infra.Data", "Repositories - CategoryRepository")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchReturnsPaginated(int quantityToGenerate, int page, int perPage, int expectedQuantityItems)
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleCategoryList = fixture.GetExampleCategoryList(quantityToGenerate);
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        var serachInput = new SearchInput(page, perPage, "", "", SearchOrder.Asc);

        var output = await categoryRepository.SearchAsync(serachInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(serachInput.Page);
        output.PerPage.Should().Be(serachInput.PerPage);
        output.Total.Should().Be(quantityToGenerate);
        output.Items.Should().HaveCount(expectedQuantityItems);

        foreach (Category outputItem in output.Items)
        {
            var exampleItem = exampleCategoryList.Find(ecl => ecl.Id == outputItem.Id);

            exampleItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem?.Description);
            outputItem.IsActive.Should().Be(exampleItem!.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }

    }

    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [Trait("Integration/Infra.Data", "Repositories - CategoryRepository")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(string search, int page, int perPage, int expectedQuantityItemsReturned, 
        int expectedQuantityTotalItems)
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleCategoryList = fixture.GetExampleCategoriesListWithName(new List<string>()
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
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        var serachInput = new SearchInput(page, perPage, search, "", SearchOrder.Asc);

        var output = await categoryRepository.SearchAsync(serachInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(serachInput.Page);
        output.PerPage.Should().Be(serachInput.PerPage);
        output.Total.Should().Be(expectedQuantityTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);

        foreach (Category outputItem in output.Items)
        {
            var exampleItem = exampleCategoryList.Find(ecl => ecl.Id == outputItem.Id);

            exampleItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem?.Description);
            outputItem.IsActive.Should().Be(exampleItem!.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }

    }
    
    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Infra.Data", "Repositories - CategoryRepository")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "desc")]
    public async Task SearchOrdered(string orderBy, string order)
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleCategoryList = fixture.GetExampleCategoryList(10);
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;

        var serachInput = new SearchInput(1, 20, "", orderBy, searchOrder);

        var output = await categoryRepository.SearchAsync(serachInput, CancellationToken.None);
        
        var expectedOrderedList = fixture.CloneCategoriesListOrdered(
            exampleCategoryList, 
            orderBy, 
            searchOrder);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(serachInput.Page);
        output.PerPage.Should().Be(serachInput.PerPage);
        output.Total.Should().Be(exampleCategoryList.Count);
        output.Items.Should().HaveCount(exampleCategoryList.Count);
        for ( int i = 0; i < expectedOrderedList.Count; i++)
        {
            var expectedItem = expectedOrderedList[i];
            var outputItem = output.Items[i];

            expectedItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem!.Id.Should().Be(expectedItem!.Id);
            outputItem!.Name.Should().Be(expectedItem!.Name);
            outputItem.Description.Should().Be(expectedItem?.Description);
            outputItem.IsActive.Should().Be(expectedItem!.IsActive);
            outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
    }
}

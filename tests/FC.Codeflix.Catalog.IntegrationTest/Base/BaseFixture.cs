using Bogus;

using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;

using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.IntegrationTest.Base;

public class BaseFixture
{
    protected Faker Faker { get; set; }

    public BaseFixture() => Faker = new Faker("pt_BR");

    public CodeflixCatalogDbContext CreateDbContext(bool preserveData = false)
    {
        var dbContext = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
            .UseInMemoryDatabase($"integration-tests-db")
            .Options
            );
        if (!preserveData)
        {
            dbContext.Database.EnsureDeleted();
        }
        return dbContext;
    }
    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
        {
            categoryName = Faker.Commerce.Categories(1)[0];
        }

        if (categoryName.Length > 255)
        {
            categoryName = categoryName[..255];
        }

        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();

        if (categoryDescription.Length > 10_000)
        {
            categoryDescription = categoryDescription[..10_000];
        }
        return categoryDescription;
    }

    public bool GetRandoBoolean()
        => (new Random()).NextDouble() < 0.5;

    public Category GetExampleCategory()
        => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandoBoolean());

    public List<Category> GetExampleCategoryList(int length = 10)
        => Enumerable.Range(1, length).Select(_ => GetExampleCategory()).ToList();

}

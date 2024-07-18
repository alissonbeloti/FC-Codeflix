using Bogus;

using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
//using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;
public class BaseFixture
{
    protected Faker Faker { get; set; }
    protected CustomWebApplicationFactory<Program> WebAppFactory { get; set; }
    protected HttpClient HttpClient { get; set; }
    public ApiClient ApiClient { get; set; }
    private readonly string? _dbConnectionString;
    public BaseFixture() 
    { 
        Faker = new Faker("pt_BR"); 
        WebAppFactory = new CustomWebApplicationFactory<Program>();
        HttpClient = WebAppFactory.CreateClient();
        ApiClient = new ApiClient(HttpClient);
        var configuration = WebAppFactory.Services.GetService(typeof(IConfiguration));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        _dbConnectionString = ((IConfiguration)configuration).GetConnectionString("CatalogDb");
    }

    public CodeflixCatalogDbContext CreateDbContext()
    {
        var dbContext = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
            .UseMySql(_dbConnectionString, ServerVersion.AutoDetect(_dbConnectionString))
            .Options
            );
        
        return dbContext;
    }

    public void CleanPersistence() 
    { 
        var context = CreateDbContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
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
}

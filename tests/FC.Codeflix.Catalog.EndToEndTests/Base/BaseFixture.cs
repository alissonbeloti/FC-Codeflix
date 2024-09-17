using Bogus;

using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;

using Keycloak.AuthServices.Authentication;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
//using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;
public class BaseFixture : IDisposable
{
    protected Faker Faker { get; set; }
    public CustomWebApplicationFactory<Program> WebAppFactory { get; set; }
    protected HttpClient HttpClient { get; set; }
    public ApiClient ApiClient { get; set; }
    private readonly string? _dbConnectionString;
    public BaseFixture() 
    { 
        Faker = new Faker("pt_BR"); 
        WebAppFactory = new CustomWebApplicationFactory<Program>();
        HttpClient = WebAppFactory.CreateClient();
        var configuration = WebAppFactory.Services.GetRequiredService<IConfiguration>();
        var keycloakOptions = configuration.GetSection(KeycloakAuthenticationOptions.Section)
            .Get<KeycloakAuthenticationOptions>();
        ApiClient = new ApiClient(HttpClient, keycloakOptions!);
        ArgumentNullException.ThrowIfNull(configuration);
        _dbConnectionString = configuration.GetConnectionString("CatalogDb");
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
    public string GetValidName()
        => Faker.Name.FullName();

    public CastMemberType GetRandomCastMemberType()
        => (CastMemberType)(new Random()).Next(1, 2);

    public string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();

        if (categoryDescription.Length > 10_000)
        {
            categoryDescription = categoryDescription[..10_000];
        }
        return categoryDescription;
    }

    public CastMember GetExampleCastMember(string? name = null)
      => new CastMember(
          name ?? GetValidName(),
          GetRandomCastMemberType());

    internal List<CastMember> GetExampleCastMemberList(int count = 10)
        => Enumerable.Range(1, count).Select(_ => GetExampleCastMember()).ToList();

    public void Dispose()
    {
        WebAppFactory.Dispose();
    }
}

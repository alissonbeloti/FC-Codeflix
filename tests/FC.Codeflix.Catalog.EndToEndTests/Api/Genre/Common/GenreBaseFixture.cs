using FC.Codeflix.Catalog.EndToEndTests.Base;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
public class GenreBaseFixture: BaseFixture
{
    public GenrePersistence Persistence;
    public CategoryPersitence CategoryPersitence;
    public GenreBaseFixture()
    {
        CodeflixCatalogDbContext dbContext = CreateDbContext();
        Persistence = new GenrePersistence(dbContext);
        CategoryPersitence = new CategoryPersitence(dbContext);
    }
    public bool GetRandoBoolean()
        => (new Random()).NextDouble() < 0.5;
    public List<Guid> GetRamdoGuids(int? count = null)
        => Enumerable.Range(1, count ?? new Random().Next(1, 10))
            .Select(_ => Guid.NewGuid())
            .ToList();

    public string GetValidGenreName()
        => Faker.Commerce.Categories(1)[0];

    public DomainEntity.Genre GetExampleGenra(bool? isActive = null,
        List<Guid>? categoriesIds = null, string? name = null)
    {
        var genre = new DomainEntity.Genre(name ?? GetValidGenreName(),
        isActive ?? GetRandoBoolean());
        categoriesIds?.ForEach(genre.AddCategory);
        return genre;
    }

    public List<DomainEntity.Genre> GetExampleGenresList(int count = 10)
        => Enumerable.Range(1, count).Select(_ =>
        {
            var genre = new DomainEntity.Genre(GetValidGenreName(),
                GetRandoBoolean());
            return genre;
        }).ToList();

    public Domain.Entity.Category GetExampleCategory()
       => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandoBoolean());

    public List<Domain.Entity.Category> GetExampleCategoryList(int length = 10)
        => Enumerable.Range(1, length).Select(_ => GetExampleCategory()).ToList();
}

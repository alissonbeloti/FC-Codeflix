using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.IntegrationTest.Base;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.IntegrationTest.Infra.Data.EF.Repositories.GenreRepository;

[CollectionDefinition(nameof(GenreRepositoryTestFixture))]
public class GenreRepositoryTestFixtureCollection : ICollectionFixture<GenreRepositoryTestFixture> { }

public class GenreRepositoryTestFixture 
    : BaseFixture
{
    public List<Guid> GetRamdoGuids(int? count = null)
        => Enumerable.Range(1, count ?? new Random().Next(1, 10))
            .Select(_ => Guid.NewGuid())
            .ToList();

    public string GetValidGenreName()
        => Faker.Commerce.Categories(1)[0];

    public DomainEntity.Genre GetExampleGenre(bool? isActive = null,
        List<Guid>? categoriesIds = null, string? name = null)
    {
        var genre = new DomainEntity.Genre(name ?? GetValidGenreName(),
        isActive ?? GetRandoBoolean());
        categoriesIds?.ForEach(genre.AddCategory);
        return genre;
    }

    

    public List<DomainEntity.Genre> GetExampleGenresList(int count = 10, bool gerarCategorias = true)
        => Enumerable.Range(1, count).Select(_ =>
        {
            var genre = new DomainEntity.Genre(GetValidGenreName(),
                GetRandoBoolean());
            if (gerarCategorias) GetRamdoGuids().ForEach(genre.AddCategory);
            return genre;
        }).ToList();

    public List<DomainEntity.Genre> GetExampleGenresListByNames(List<string> names)
        => names.Select(name => GetExampleGenre(name: name)).ToList();

    public List<Genre> CloneGenresListOrdered(List<Genre> genres, string orderBy, SearchOrder order)
    {
        var listClone = new List<Genre>(genres);
        var orderedEnumerable = (orderBy.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => listClone.OrderBy(x => x.Name).ThenBy(x => x.Id),
            ("name", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Name)
                .ThenByDescending(x => x.Id),
            ("id", SearchOrder.Asc) => listClone.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
            _ => listClone.OrderBy(x => x.Name)
                .ThenBy(x => x.Id),
        };
        return orderedEnumerable.ToList();
    }
}

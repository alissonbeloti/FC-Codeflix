using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Genre.Common;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Genre.ListGenre;

[CollectionDefinition(nameof(ListGenreTestFixture))]
public class ListGenreTestFixtureCollection : ICollectionFixture<ListGenreTestFixture> { }

public class ListGenreTestFixture: GenreUseCasesBaseFixture
{
    public List<Domain.Entity.Genre> CloneGenresListOrdered(List<Domain.Entity.Genre> genres, string orderBy, SearchOrder order)
    {
        var listClone = new List<Domain.Entity.Genre>(genres);
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

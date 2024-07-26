
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.ListGenres;
[CollectionDefinition(nameof(ListGenresApiTestFixture))]
public class ListGenresApiTestFixtureCollection : ICollectionFixture<ListGenresApiTestFixture> { }
public class ListGenresApiTestFixture: GenreBaseFixture
{
    public List<Domain.Entity.Genre> GetExampleGenresListByNames(List<string> names)
      => names.Select(name => GetExampleGenra(name: name)).ToList();
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

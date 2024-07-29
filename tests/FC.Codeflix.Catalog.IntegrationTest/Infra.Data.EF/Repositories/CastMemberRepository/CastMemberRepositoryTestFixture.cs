using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.IntegrationTest.Base;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.IntegrationTest.Infra.Data.EF.Repositories.CastMemberRepository;
[CollectionDefinition(nameof(CastMemberRepositoryTestFixture))]
public class CastMemberRepositoryTestFixtureCollection : ICollectionFixture<CastMemberRepositoryTestFixture> { }
public class CastMemberRepositoryTestFixture : BaseFixture
{
    internal List<CastMember> GetExampleCastMemberListByNames(List<string> names)
        => names.Select(GetExampleCastMember).ToList();

    public List<CastMember> CloneCastMemberListOrdered(List<CastMember> genres, string orderBy, SearchOrder order)
    {
        var listClone = new List<CastMember>(genres);
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

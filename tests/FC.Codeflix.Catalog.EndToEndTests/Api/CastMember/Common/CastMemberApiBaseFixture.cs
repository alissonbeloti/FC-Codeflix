using FC.Codeflix.Catalog.EndToEndTests.Base;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;
[CollectionDefinition(nameof(CastMemberApiBaseFixture))]
public class CastMemberApiBaseFixtureCollection : ICollectionFixture<CastMemberApiBaseFixture> { }
public class CastMemberApiBaseFixture: BaseFixture
{
    public CastMemberPersistence Persistence;

    public CastMemberApiBaseFixture()
    {
        Persistence = new CastMemberPersistence(CreateDbContext());
    }
    internal List<DomainEntity.CastMember> GetExampleCastMemberListByNames(List<string> names)
        => names.Select(GetExampleCastMember).ToList();

    public List<DomainEntity.CastMember> CloneCastMemberListOrdered(List<DomainEntity.CastMember> genres, string orderBy, SearchOrder order)
    {
        var listClone = new List<DomainEntity.CastMember>(genres);
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

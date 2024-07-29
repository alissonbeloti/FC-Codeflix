using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.IntegrationTest.Base;

using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.CastMember.Common;

[CollectionDefinition(nameof(CastMemberUseCaseBaseFixture))]
public class CastMemberUseCaseBaseFixtureCollection : ICollectionFixture<CastMemberUseCaseBaseFixture> { }

public class CastMemberUseCaseBaseFixture: BaseFixture
{
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

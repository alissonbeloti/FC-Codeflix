using FC.Codeflix.Catalog.UnitTests.Application.CastMember.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.List;
[CollectionDefinition(nameof(ListCastMembersFixture))]
public class ListCastMembersFixtureCollection : ICollectionFixture<ListCastMembersFixture> { }
public class ListCastMembersFixture : CastMemberUsecasesBaseFixture
{
    internal List<DomainEntity.CastMember> GetExampleCastMemberList(int count)
        => Enumerable.Range(1, count).Select(_ => GetExampleCastMember()).ToList();
    
}

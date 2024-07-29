using FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.CastMember.Common;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.CastMember.Create;

[CollectionDefinition(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTestFixtureCollection: ICollectionFixture<CreateCastMemberTestFixture> { }

public class CreateCastMemberTestFixture : CastMemberUseCaseBaseFixture
{
}

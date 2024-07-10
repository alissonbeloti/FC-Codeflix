using FC.Codeflix.Catalog.IntegrationTests.Base;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.UnityOfWork;
[CollectionDefinition(nameof(UnitOfWorkTestFixture))]
public class UnitOfWorkTestFixtureColletion : ICollectionFixture<UnitOfWorkTestFixture>
{ }
public class UnitOfWorkTestFixture: BaseFixture
{

}

using FC.Codeflix.Catalog.IntegrationTest.Base;

namespace FC.Codeflix.Catalog.IntegrationTest.Infra.Data.EF.UnityOfWork;
[CollectionDefinition(nameof(UnitOfWorkTestFixture))]
public class UnitOfWorkTestFixtureColletion : ICollectionFixture<UnitOfWorkTestFixture>
{ }
public class UnitOfWorkTestFixture: BaseFixture
{

}

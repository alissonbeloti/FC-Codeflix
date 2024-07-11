using FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Category.Common;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Category.DeleteCategory;
[CollectionDefinition(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTestFixtureCollection : ICollectionFixture<DeleteCategoryTestFixture> 
{}
public class DeleteCategoryTestFixture: CategoryUseCaseFixture
{

}

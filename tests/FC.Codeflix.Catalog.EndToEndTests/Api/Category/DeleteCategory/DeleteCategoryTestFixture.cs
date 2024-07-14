using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.DeleteCategory;
[CollectionDefinition(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTestFixtureCollection :
    ICollectionFixture<DeleteCategoryTestFixture>
{ }
public class DeleteCategoryTestFixture: CategoryBaseFixture
{

}

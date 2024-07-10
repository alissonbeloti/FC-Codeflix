using FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Category.Common;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Category.GetCategory;

[CollectionDefinition(nameof(GetCategoryTestFixture))]
public class GetCategoryTestFixtureCollection
    : ICollectionFixture<GetCategoryTestFixture>
{ }

public class GetCategoryTestFixture : CategoryUseCaseFixture
{

}

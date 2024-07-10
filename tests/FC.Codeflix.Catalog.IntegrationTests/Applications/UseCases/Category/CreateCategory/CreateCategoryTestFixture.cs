using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.IntegrationTests.Applications.UseCases.Category.Common;

namespace FC.Codeflix.Catalog.IntegrationTests.Applications.UseCases.Category.CreateCategory;

[CollectionDefinition(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTestFixtureCollection :
    ICollectionFixture<CreateCategoryTestFixture>
{ }

public class CreateCategoryTestFixture
    : CategoryUseCaseFixture
{
    public CreateCategoryInput GetInput()
        => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandoBoolean());
}

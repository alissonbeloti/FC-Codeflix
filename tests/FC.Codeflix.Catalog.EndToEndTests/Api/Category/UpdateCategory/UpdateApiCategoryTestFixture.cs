using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory;
[CollectionDefinition(nameof(UpdateApiCategoryTestFixture))]
public class UpdateApiCategoryTestFixtureCollection : ICollectionFixture<UpdateApiCategoryTestFixture>
{ }
public class UpdateApiCategoryTestFixture : CategoryBaseFixture
{
    public UpdateCategoryInput GetExampleInput(Guid? categoryId = null)
    {
        return new UpdateCategoryInput
        (
            categoryId ?? Guid.NewGuid(),
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandoBoolean()
        );
    }
}

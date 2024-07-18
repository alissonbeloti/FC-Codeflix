using FC.Codeflix.Catalog.Api.ApiModels.Category;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory;
[CollectionDefinition(nameof(UpdateApiCategoryTestFixture))]
public class UpdateApiCategoryTestFixtureCollection : ICollectionFixture<UpdateApiCategoryTestFixture>
{ }
public class UpdateApiCategoryTestFixture : CategoryBaseFixture
{
    public UpdateCategoryApiInput GetExampleInput()
    {
        return new UpdateCategoryApiInput
        (
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandoBoolean()
        );
    }
}

using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Category.Common;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Category.UpdateCategory;
[CollectionDefinition(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTestFixtureCollection :
    ICollectionFixture<UpdateCategoryTestFixture>
{ }
public class UpdateCategoryTestFixture
    : CategoryUseCaseFixture
{
    public UpdateCategoryInput GetValidInput(Guid? id = null)
        => new(id ?? Guid.NewGuid(), GetValidCategoryName(), GetValidCategoryDescription(), GetRandoBoolean());

    public UpdateCategoryInput GetInvalidInputShortName()
    {
        var invalidInputShortName = GetValidInput();
        invalidInputShortName.Name = invalidInputShortName.Name.Substring(0, 2);
        return invalidInputShortName;

    }

    internal UpdateCategoryInput GetInvalidInputTooLongDescritpion()
    {
        var invalidInputToLongDescription = GetValidInput();
        var tooLongDescriptionForCategory = Faker.Commerce.ProductName();

        while (tooLongDescriptionForCategory.Length <= 10_000)
            tooLongDescriptionForCategory = $"{tooLongDescriptionForCategory} {Faker.Commerce.ProductName()}";

        invalidInputToLongDescription.Description = tooLongDescriptionForCategory;
        return invalidInputToLongDescription;
    }

    internal UpdateCategoryInput GetInvalidInputLongName()
    {
        var invalidInputToLongName = GetValidInput();
        var tooLongNameForCategory = Faker.Commerce.ProductName();

        while (tooLongNameForCategory.Length <= 255)
            tooLongNameForCategory = $"{tooLongNameForCategory} {Faker.Commerce.ProductName()}";

        invalidInputToLongName.Name = tooLongNameForCategory;
        return invalidInputToLongName;
    }
}

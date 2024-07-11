using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Category.Common;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Category.CreateCategory;

[CollectionDefinition(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTestFixtureCollection :
    ICollectionFixture<CreateCategoryTestFixture>
{ }

public class CreateCategoryTestFixture
    : CategoryUseCaseFixture
{
    public CreateCategoryInput GetInput()
        => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandoBoolean());
    public CreateCategoryInput GetInvalidInputShortName()
    {
        var invalidInputShortName = GetInput();
        invalidInputShortName.Name = invalidInputShortName.Name.Substring(0, 2);
        return invalidInputShortName;

    }
    internal CreateCategoryInput GetInvalidInputDescriptionNull()
    {
        var invalidInputToDescriptionNull = GetInput();
        invalidInputToDescriptionNull.Description = null;

        return invalidInputToDescriptionNull;
    }

    internal CreateCategoryInput GetInvalidInputTooLongDescritpion()
    {
        var invalidInputToLongDescription = GetInput();
        var tooLongDescriptionForCategory = Faker.Commerce.ProductName();

        while (tooLongDescriptionForCategory.Length <= 10_000)
            tooLongDescriptionForCategory = $"{tooLongDescriptionForCategory} {Faker.Commerce.ProductName()}";

        invalidInputToLongDescription.Description = tooLongDescriptionForCategory;
        return invalidInputToLongDescription;
    }

    internal CreateCategoryInput GetInvalidInputLongName()
    {
        var invalidInputToLongName = GetInput();
        var tooLongNameForCategory = Faker.Commerce.ProductName();

        while (tooLongNameForCategory.Length <= 255)
            tooLongNameForCategory = $"{tooLongNameForCategory} {Faker.Commerce.ProductName()}";

        invalidInputToLongName.Name = tooLongNameForCategory;
        return invalidInputToLongName;
    }
}

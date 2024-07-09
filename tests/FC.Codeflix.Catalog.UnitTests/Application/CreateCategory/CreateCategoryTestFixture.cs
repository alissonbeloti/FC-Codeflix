using Moq;
using FC.Codeflix.Catalog.UnitTests.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application.CreateCategory;
[CollectionDefinition(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTestFixtureCollection:
    ICollectionFixture<CreateCategoryTestFixture>
{

}
public class CreateCategoryTestFixture: BaseFixture
{
    public CreateCategoryTestFixture() : base() { }
    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
        {
            categoryName = Faker.Commerce.Categories(1)[0];
        }

        if (categoryName.Length > 255)
        {
            categoryName = categoryName[..255];
        }

        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();

        if (categoryDescription.Length > 10_000)
        {
            categoryDescription = categoryDescription[..10_000];
        }
        return categoryDescription;
    }

    public bool GetRandoBoolean()
        => (new Random()).NextDouble() < 0.5;

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

    public Mock<ICategoryRepository> GetRepositoryMock => new();
    public Mock<IUnitOfWork> GetUnitOfWorkMock => new();

}

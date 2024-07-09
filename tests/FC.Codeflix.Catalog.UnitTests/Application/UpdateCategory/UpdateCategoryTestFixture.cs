using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Moq;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application.UpdateCategory;

[CollectionDefinition(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTestFixtureCollection
    : ICollectionFixture<UpdateCategoryTestFixture>
{ }
public class UpdateCategoryTestFixture : BaseFixture
{
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
        var categoryDescription = Faker.Commerce.ProductDescription(); ;

        if (categoryDescription.Length > 10_000)
        {
            categoryDescription = categoryDescription[..10_000];
        }
        return categoryDescription;
    }
    public bool GetRandoBoolean()
        => (new Random()).NextDouble() < 0.5;
    public DomainEntity.Category GetExampleCategory()
        => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandoBoolean());

    public Mock<ICategoryRepository> GetRepositoryMock() => new();
    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();

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

using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Common;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.ListCategories;
[CollectionDefinition(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTestFixtureCollection :
    ICollectionFixture<ListCategoriesTestFixture>
{ }
public class ListCategoriesTestFixture
    : BaseFixture
{
    internal Mock<ICategoryRepository> GetRepositoryMock() 
        => new();

    internal string GetValidCategoryName()
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

    internal string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription(); ;

        if (categoryDescription.Length > 10_000)
        {
            categoryDescription = categoryDescription[..10_000];
        }
        return categoryDescription;
    }

    internal bool GetRandoBoolean()
        => (new Random()).NextDouble() < 0.5;

    internal Category GetExampleCategory()
        => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandoBoolean());

    public List<Category> GetExampleCategoriesList(int len = 10)
    {
        var list = new List<Category>();
        for (int i = 0; i < len; i++)
        {
            list.Add(GetExampleCategory());
        }
        return list;
    }

}

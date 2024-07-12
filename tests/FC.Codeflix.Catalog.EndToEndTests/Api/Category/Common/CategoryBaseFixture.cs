using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.EndToEndTests.Base;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;

public class CategoryBaseFixture : BaseFixture
{
    public CategoryBaseFixture()
    {
        Persistence = new CategoryPersitence(CreateDbContext());
    }
    public bool GetRandoBoolean()
        => (new Random()).NextDouble() < 0.5;

    public CategoryPersitence Persistence;

    public string GetInvalidShortName()
    {
        var invalidInputShortName = Faker.Commerce.ProductName();
        return invalidInputShortName.Substring(0, 2);
    }
    
    internal string GetInvalidTooLongDescription()
    {
        var tooLongDescriptionForCategory = Faker.Commerce.ProductName();
        while (tooLongDescriptionForCategory.Length <= 10_000)
            tooLongDescriptionForCategory = $"{tooLongDescriptionForCategory} {Faker.Commerce.ProductName()}";
        return tooLongDescriptionForCategory;
    }

    internal string GetInvalidLongName()
    {
        var tooLongNameForCategory = Faker.Commerce.ProductName();
        while (tooLongNameForCategory.Length <= 255)
            tooLongNameForCategory = $"{tooLongNameForCategory} {Faker.Commerce.ProductName()}";
        return tooLongNameForCategory;
    }
}

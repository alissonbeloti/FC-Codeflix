using FC.Codeflix.Catalog.UnitTests.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Genre;
[CollectionDefinition(nameof(GenreTestFixture))]
public class GenreTestFixtureFixture: ICollectionFixture<GenreTestFixture>
{}
public class GenreTestFixture :
    BaseFixture
{
    public string GetValidName()
    {
        var name = "";
        while (name.Length < 3)
        {
            name = Faker.Commerce.Categories(1)[0];
        }

        if (name.Length > 255)
        {
            name = name[..255];
        }

        return name;
    }


    public DomainEntity.Genre GetValidGenre(bool isActive = true,
        List<Guid>? categoriesList = null)
    { 
        DomainEntity.Genre genre = new(GetValidName(), isActive);
        if (categoriesList != null) 
        {
            foreach (var id in categoriesList)
            {
                genre.AddCategory(id);
            }
        }
        return genre;
    }
}

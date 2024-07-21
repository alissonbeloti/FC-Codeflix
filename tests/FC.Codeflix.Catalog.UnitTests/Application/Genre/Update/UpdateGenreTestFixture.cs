using FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using FC.Codeflix.Catalog.UnitTests.Application.Genre.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.Update;

[CollectionDefinition(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTestFixtureColletion: ICollectionFixture<UpdateGenreTestFixture> { }

public class UpdateGenreTestFixture: GenreUscasesBaseFixture
{
    public DomainEntity.Genre GetExampleGenra(bool? isActive = null,
        List<Guid>? categoriesIds = null)
    {
        var genre = new DomainEntity.Genre(GetValidGenreName(),
        isActive ?? GetRandoBoolean());
        categoriesIds?.ForEach(genre.AddCategory);
        return genre;
    }
}

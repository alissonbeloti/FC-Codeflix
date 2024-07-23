using FC.Codeflix.Catalog.UnitTests.Application.Genre.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.Update;

[CollectionDefinition(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTestFixtureColletion: ICollectionFixture<UpdateGenreTestFixture> { }

public class UpdateGenreTestFixture: GenreUsecasesBaseFixture
{
}

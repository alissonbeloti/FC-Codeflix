using FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Genre.Common;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Genre.UpdateGenre;

[CollectionDefinition(nameof(UpdateGenreTestFixture))]
public class UdateGenreTestFixtureCollection : ICollectionFixture<UpdateGenreTestFixture> { }
public class UpdateGenreTestFixture: GenreUseCasesBaseFixture
{

}

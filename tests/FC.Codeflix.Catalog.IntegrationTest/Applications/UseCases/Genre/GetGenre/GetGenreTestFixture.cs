using FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Genre.Common;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Genre.GetGenre;
[CollectionDefinition(nameof(GetGenreTestFixture))]
public class GetGenreTestFixtureCollection : ICollectionFixture<GetGenreTestFixture> { }
public class GetGenreTestFixture: GenreUseCasesBaseFixture
{

}

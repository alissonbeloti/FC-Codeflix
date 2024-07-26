using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.CreateGenre;
[CollectionDefinition(nameof(CreateGenreApiTestFixture))]
public class CreateGenreApiTestFixtureCollection : ICollectionFixture<CreateGenreApiTestFixture> { }
public class CreateGenreApiTestFixture: GenreBaseFixture
{

}

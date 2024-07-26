using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.UpdateGenre;
[CollectionDefinition(nameof(UpdateGenreApiTestFixture))]
public class UpdateGenreApiTestFixtureCollection : ICollectionFixture<UpdateGenreApiTestFixture> { }
public class UpdateGenreApiTestFixture: GenreBaseFixture
{
}

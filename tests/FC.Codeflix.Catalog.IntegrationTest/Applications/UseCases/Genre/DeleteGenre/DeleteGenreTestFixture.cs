using FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Genre.Common;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Genre.DeleteGenre;
[CollectionDefinition(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTestFixtureCollection : ICollectionFixture<DeleteGenreTestFixture> { }

public class DeleteGenreTestFixture: GenreUseCasesBaseFixture
{

}

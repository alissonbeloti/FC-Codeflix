using FC.Codeflix.Catalog.UnitTests.Application.Genre.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.Delete;
[CollectionDefinition(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTestFixtureCollection : ICollectionFixture<DeleteGenreTestFixture> { }
public class DeleteGenreTestFixture : GenreUsecasesBaseFixture
{
}

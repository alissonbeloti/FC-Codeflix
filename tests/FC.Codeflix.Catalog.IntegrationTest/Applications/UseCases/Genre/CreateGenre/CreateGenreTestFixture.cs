using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Genre.Common;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Genre.CreateGenre;
[CollectionDefinition(nameof(CreateGenreTestFixture))]
public class CreateGenreTestFixtureCollection : ICollectionFixture<CreateGenreTestFixture> { }

public class CreateGenreTestFixture : GenreUseCasesBaseFixture
{
    public CreateGenreInput GetExampleInput()
        => new(GetValidGenreName(), GetRandoBoolean());
   
}

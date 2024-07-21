using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.Codeflix.Catalog.UnitTests.Application.Genre.Common;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.Create;

[CollectionDefinition(nameof(CreateGenreTestFixture))]
public class CreateGenreTestFixtureCollection : ICollectionFixture<CreateGenreTestFixture> { }

public class CreateGenreTestFixture : GenreUscasesBaseFixture
{
        
    public CreateGenreInput GetExampleInput()
       => new(GetValidGenreName(), GetRandoBoolean());
    public CreateGenreInput GetExampleInputWithCategories()
    {
        var numberOfCategories = (new Random()).Next(1, 10);
        var categoriesIds = Enumerable.Range(0, numberOfCategories)
            .Select(_ => Guid.NewGuid()).ToList();
        return new(GetValidGenreName(), GetRandoBoolean(), categoriesIds);
    }
    
}

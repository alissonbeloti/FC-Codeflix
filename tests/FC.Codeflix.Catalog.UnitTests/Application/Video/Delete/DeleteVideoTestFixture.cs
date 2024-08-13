using FC.Codeflix.Catalog.Application.UseCases.Video.DeleteVideo;
using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.Delete;
[CollectionDefinition(nameof(DeleteVideoTestFixture))]
public class DeleteVideoTestFixtureCollection : ICollectionFixture<DeleteVideoTestFixture> { }
public class DeleteVideoTestFixture : VideoTestFixtureBase
{
    internal DeleteVideoInput GetValidInput(Guid? id = null)
        => new(id ?? Guid.NewGuid());
    
}

using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.Get;

[CollectionDefinition(nameof(GetVideoTestFixture))]
public class GetVideoTestFixtureCollection : ICollectionFixture<GetVideoTestFixture> { }

public class GetVideoTestFixture: VideoTestFixtureBase
{

}

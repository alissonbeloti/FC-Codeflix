using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;

using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias;
namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UploadMedias;

[CollectionDefinition(nameof(UploadMediaTestFixture))]
public class UploadMediaTestFixtureCollection : ICollectionFixture<UploadMediaTestFixture> { }
public class UploadMediaTestFixture : VideoTestFixtureBase
{
    internal UseCase.UploadMediasInput GetValidInput(Guid? videoId = null,
        bool withVideoFile = true,
        bool withTrailerFile = true) =>
        new UseCase.UploadMediasInput(
            videoId ?? Guid.NewGuid(),
            withVideoFile? GetValidMediaFileInput(): null,
            withTrailerFile? GetValidMediaFileInput(): null);
    
}

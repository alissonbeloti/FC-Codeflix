using FC.Codeflix.Catalog.Application.UseCases.Video.UpdateMediaStatus;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UpdateMediaStatus;
[CollectionDefinition(nameof(UpdateMediaStatusTestFixture))]
public class UpdateMediaStatusTestFixtureCollection : ICollectionFixture<UpdateMediaStatusTestFixture> { }

public class UpdateMediaStatusTestFixture: VideoTestFixtureBase
{
    public UpdateMediaStatusInput GetSuccededEncodingInput(Guid videoId) =>
        new (
            videoId,
            MediaStatus.Completed,
            EncodedPath: GetValidMediaPath());

    public UpdateMediaStatusInput GetFailedEncodingInput(Guid videoId) =>
        new(
            videoId,
            MediaStatus.Error,
            ErrorMessage: "There was an error while trying to encode video.");

    public UpdateMediaStatusInput GetInvalidStatusEncodingInput(Guid videoId) =>
        new(
            videoId,
            MediaStatus.Processing,
            ErrorMessage: "There was an error while trying to encode video.");
}

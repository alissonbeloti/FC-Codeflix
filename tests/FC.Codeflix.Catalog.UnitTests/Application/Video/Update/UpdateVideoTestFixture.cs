using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;
namespace FC.Codeflix.Catalog.UnitTests.Application.Video.Update;

[CollectionDefinition(nameof(UpdateVideoTestFixture))]
public class UpdateVideoTestFixtureCollection : ICollectionFixture<UpdateVideoTestFixture> { }
public class UpdateVideoTestFixture : VideoTestFixtureBase
{
    internal UseCase.UpdateVideoInput CreateValidInput(Guid videoId,
        List<Guid>? genreIds = null,
        List<Guid>? categoryIds = null,
        List<Guid>? castMemberIds = null,
        FileInput? banner = null,
        FileInput? thumb = null,
        FileInput? thumbHalf = null)
        => new(
            videoId,
            GetValidTitle(),
            GetValidDescription(),
            GetRandomRationg(),
            GetValidYearLaunched(),
            GetRandoBoolean(),
            GetValidDuration(),
            GetRandoBoolean(),
            categoryIds,
            genreIds,
            castMemberIds,
            Banner: banner,
            Thumb: thumb,
            ThumbHalf: thumbHalf
            );
}

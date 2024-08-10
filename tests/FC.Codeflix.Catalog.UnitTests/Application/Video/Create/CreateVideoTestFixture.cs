using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;
using System.Text;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.Create;
[CollectionDefinition(nameof(CreateVideoTestFixture))]
public class CreateVideoTestFixtureCollection : ICollectionFixture<CreateVideoTestFixture> { }

public class CreateVideoTestFixture : VideoTestFixtureBase
{
    internal CreateVideoInput CreateValidCreateVideoInput(
        List<Guid>? categoriesIds = null,
        List<Guid>? genresIds = null,
        List<Guid>? castMembersIds = null,
        FileInput? thumb = null,
        FileInput? banner = null,
        FileInput? thumbHalf = null
        ) =>
        new (
            GetValidTitle(),
            GetValidDescription(),
            GetRandomRationg(),
            GetValidYearLaunched(),
            GetRandoBoolean(),
            GetValidDuration(),
            GetRandoBoolean(),
            categoriesIds,
            genresIds,
            castMembersIds,
            thumb,
            banner,
            thumbHalf
        );
    internal CreateVideoInput CreateValidInputAllData(
        List<Guid>? categoriesIds = null,
        List<Guid>? genresIds = null,
        List<Guid>? castMembersIds = null,
        FileInput? thumb = null,
        FileInput? banner = null,
        FileInput? thumbHalf = null
        ) =>
        new(
            GetValidTitle(),
            GetValidDescription(),
            GetRandomRationg(),
            GetValidYearLaunched(),
            GetRandoBoolean(),
            GetValidDuration(),
            GetRandoBoolean(),
            Enumerable.Range(1,5).Select(_ => Guid.NewGuid()).ToList(),
            Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList(),
            Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList(),
            GetValidImageFileInput(),
            GetValidImageFileInput(),
            GetValidImageFileInput()
        );
}

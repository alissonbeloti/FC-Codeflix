using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;

using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Video;
[Collection(nameof(VideoTestFixture))]
public class MediaTest(VideoTestFixture fixture)
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain","Media - Entities")]
    public void Instantiate()
    {
        var expectedFilePath = fixture.GetValidMediaPath();

        var media = new Media(expectedFilePath);

        media.FilePath.Should().Be(expectedFilePath);
        media.Status.Should().Be(MediaStatus.Pending);
    }

    [Fact(DisplayName = nameof(UpdateAsSentToEncode))]
    [Trait("Domain", "Media - Entities")]
    public void UpdateAsSentToEncode()
    {
        var media = fixture.GetValidMedia();

        media.UpdateAsSentToEncode();

        media.Status.Should().Be(MediaStatus.Processing);
    }

    [Fact(DisplayName = nameof(UpdateAsEncoded))]
    [Trait("Domain", "Media - Entities")]
    public void UpdateAsEncoded()
    {
        var media = fixture.GetValidMedia();
        var encodedPath = fixture.GetValidMediaPath();

        media.UpdateAsEncoded(encodedPath);

        media.Status.Should().Be(MediaStatus.Completed);
        media.EncondedPath.Should().Be(encodedPath);
    }


}

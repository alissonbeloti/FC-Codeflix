using Moq;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Repository;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.GetVideo;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Extensions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.Get;
[Collection(nameof(GetVideoTestFixture))]
public class GetVideoTest(GetVideoTestFixture fixture)
{

    [Fact(DisplayName = nameof(GetVideo))]
    [Trait("Application", "GetVideo - Use Cases")]
    public async Task GetVideo()
    {
        var exampleVideo = fixture.GetValidVideo();
        var repositoryMock = new Mock<IVideoRepository>();
        repositoryMock.Setup(x => x.Get(exampleVideo.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        var useCase = new UseCase.GetVideo(repositoryMock.Object); 
        var input = new UseCase.GetVideoInput(exampleVideo.Id);
        
        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(exampleVideo.Title);
        output.Description.Should().Be(exampleVideo.Description);
        output.Opened.Should().Be(exampleVideo.Opened);
        output.Published.Should().Be(exampleVideo.Published);
        output.Duration.Should().Be(exampleVideo.Duration);
        output.Rating.Should().Be(exampleVideo.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
    }

    [Fact(DisplayName = nameof(ThrowsExceptionWhenNotFound))]
    [Trait("Application", "GetVideo - Use Cases")]
    public async Task ThrowsExceptionWhenNotFound()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Video not found"));
        var useCase = new UseCase.GetVideo(repositoryMock.Object);
        var input = new UseCase.GetVideoInput(Guid.NewGuid());

        var action = () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Video not found");
        repositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(GetVideoWithAllProperties))]
    [Trait("Application", "GetVideo - Use Cases")]
    public async Task GetVideoWithAllProperties()
    {
        var exampleVideo = fixture.GetValidVideoWithAllProperties();
        var repositoryMock = new Mock<IVideoRepository>();
        repositoryMock.Setup(x => x.Get(exampleVideo.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        var useCase = new UseCase.GetVideo(repositoryMock.Object);
        var input = new UseCase.GetVideoInput(exampleVideo.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(exampleVideo.Title);
        output.Description.Should().Be(exampleVideo.Description);
        output.Opened.Should().Be(exampleVideo.Opened);
        output.Published.Should().Be(exampleVideo.Published);
        output.Duration.Should().Be(exampleVideo.Duration);
        output.Rating.Should().Be(exampleVideo.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.ThumbFileUrl.Should().Be(exampleVideo.Thumb!.Path);
        output.ThumbHalfFileUrl.Should().Be(exampleVideo.ThumbHalf!.Path);
        output.BannerFileUrl.Should().Be(exampleVideo.Banner!.Path);
        output.VideoFileUrl.Should().Be(exampleVideo.Media!.FilePath);
        output.TrailerFileUrl.Should().Be(exampleVideo.Trailer!.FilePath);
        output.Categories!.Select(dto => dto.Id).Should().BeEquivalentTo(exampleVideo.Categories);
        output.CastMembers!.Select(dto => dto.Id).Should().BeEquivalentTo(exampleVideo.CastMembers);
        output.Genres!.Select(dto => dto.Id).Should().BeEquivalentTo(exampleVideo.Genres);
    }
}

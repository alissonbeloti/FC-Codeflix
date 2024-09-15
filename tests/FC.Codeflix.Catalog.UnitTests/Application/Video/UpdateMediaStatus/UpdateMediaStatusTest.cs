using Moq;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.UpdateMediaStatus;
using FC.Codeflix.Catalog.Domain.Extensions;
using FC.Codeflix.Catalog.Domain.Enum;
using Microsoft.Extensions.Logging;
using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UpdateMediaStatus;
[Collection(nameof(UpdateMediaStatusTestFixture))]
public class UpdateMediaStatusTest
{
    private readonly Mock<IVideoRepository> _videoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly UseCase.UpdateMediaStatus _useCase;
    private readonly UpdateMediaStatusTestFixture _fixture;

    public UpdateMediaStatusTest(UpdateMediaStatusTestFixture fixture)
    {
        _fixture = fixture;
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _useCase = new UseCase.UpdateMediaStatus(_videoRepositoryMock.Object, 
            _unitOfWork.Object,
            Mock.Of<ILogger<UseCase.UpdateMediaStatus>>());
    }

    [Fact(DisplayName = nameof(HandleWhenSucceededEncoding))]
    [Trait("Application", "UpdateMediaStatus - Use Cases")]
    public async Task HandleWhenSucceededEncoding()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.GetSuccededEncodingInput(exampleVideo.Id);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(exampleVideo.Title);
        output.Description.Should().Be(exampleVideo.Description);
        output.Opened.Should().Be(exampleVideo.Opened);
        output.Duration.Should().Be(exampleVideo.Duration);
        output.Rating.Should().Be(exampleVideo.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        exampleVideo.Media!.Status.Should().Be(MediaStatus.Completed);
        exampleVideo.Media.EncondedPath.Should().Be(input.EncodedPath);
        _videoRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(x => x.Update(exampleVideo, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(HandleWhenFailedEncoding))]
    [Trait("Application", "UpdateMediaStatus - Use Cases")]
    public async Task HandleWhenFailedEncoding()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.GetFailedEncodingInput(exampleVideo.Id);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(exampleVideo.Title);
        output.Description.Should().Be(exampleVideo.Description);
        output.Opened.Should().Be(exampleVideo.Opened);
        output.Duration.Should().Be(exampleVideo.Duration);
        output.Rating.Should().Be(exampleVideo.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        exampleVideo.Media!.Status.Should().Be(MediaStatus.Error);
        exampleVideo.Media.EncondedPath.Should().BeNull();
        _videoRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(x => x.Update(exampleVideo, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(HandleWhenInvalidInput))]
    [Trait("Application", "UpdateMediaStatus - Use Cases")]
    public async Task HandleWhenInvalidInput()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var expectedEncodePath = exampleVideo.Media!.EncondedPath;
        var expectedErrorMessage = "Invalid media status";
        var input = _fixture.GetInvalidStatusEncodingInput(exampleVideo.Id);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var action = async() => await _useCase.Handle(input, CancellationToken.None);

        action.Should().NotBeNull();
        await action.Should().ThrowAsync<EntityValidationException>()
            .WithMessage(expectedErrorMessage);
        exampleVideo.Media!.Status.Should().Be(MediaStatus.Pending);
        exampleVideo.Media.EncondedPath.Should().Be(expectedEncodePath);
        _videoRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(x => x.Update(exampleVideo, It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }
}

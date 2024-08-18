using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias;
using Moq;
using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UploadMedias;

[Collection(nameof(UploadMediaTestFixture))]
public class UploadMediaTest
{
    private readonly UploadMediaTestFixture _fixture;
    private readonly UseCase.UploadMedias _useCase;
    private readonly Mock<IVideoRepository> _videoRepositoryMock;
    private readonly Mock<IUnitOfWork> _UnitOfWorkMock;
    private readonly Mock<IStorageService> _StorageServiceMock;

    public UploadMediaTest(UploadMediaTestFixture fixture)
    {
        _fixture = fixture;
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _UnitOfWorkMock = new Mock<IUnitOfWork>();
        _StorageServiceMock = new Mock<IStorageService>();
        _useCase = new UseCase.UploadMedias(
            _videoRepositoryMock.Object,
            _UnitOfWorkMock.Object,
            _StorageServiceMock.Object
        );
    }

    [Fact(DisplayName = nameof(UploadMedia))]
    [Trait("Application", "UploadMedias - Use Cases")]
    public async Task UploadMedia()
    {
        var video = _fixture.GetValidVideo();
        var validInput = _fixture.GetValidInput(video.Id);
        var fileNames = new List<string> {
            StorageName.Create(video.Id, nameof(video.Media),
            validInput.VideoInput!.Extension),
            StorageName.Create(video.Id, nameof(video.Trailer),
            validInput.TrailerInput!.Extension)
        };
        _videoRepositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == video.Id),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(video);
        _StorageServiceMock.Setup(x => x.Upload(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid().ToString());

        await _useCase.Handle(validInput, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _StorageServiceMock.Verify(x => x.Upload(
            It.Is<string>(x => fileNames.Contains(x)),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()),
            Times.Exactly(2));
        _UnitOfWorkMock.Verify(x => x.Commit(CancellationToken.None));
    }

    [Fact(DisplayName = nameof(ClearStorageInUploadErrorCase))]
    [Trait("Application", "UploadMedias - Use Cases")]
    public async Task ClearStorageInUploadErrorCase()
    {
        var video = _fixture.GetValidVideo();
        var validInput = _fixture.GetValidInput(video.Id);
        var videoFileName = StorageName.Create(video.Id, nameof(video.Media),
            validInput.VideoInput!.Extension);
        var trailerFileName = StorageName.Create(video.Id, nameof(video.Trailer),
            validInput.TrailerInput!.Extension);
        var videoStoragePath = $"storage/{videoFileName}";
        var trailerStoragePath = $"storage/{trailerFileName}";
        var fileNames = new List<string> { videoFileName, trailerFileName };
        _videoRepositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == video.Id),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(video);
        _StorageServiceMock.Setup(x => x.Upload(
            It.Is<string>(x => x == videoFileName),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(videoStoragePath);
        _StorageServiceMock.Setup(x => x.Upload(
            It.Is<string>(x => x == trailerFileName),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong with the upload"));

        var action = () => _useCase.Handle(validInput, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Something went wrong with the upload");

        _videoRepositoryMock.VerifyAll();
        _StorageServiceMock.Verify(x => x.Upload(
            It.Is<string>(x => fileNames.Contains(x)),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()),
            Times.Exactly(2));
        _StorageServiceMock.Verify(x => x.Delete(
            It.Is<string>(fileName => fileName == videoStoragePath),
            It.IsAny<CancellationToken>()), Times.Exactly(1));
        _StorageServiceMock.Verify(x => x.Delete(
           It.IsAny<string>(),
           It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact(DisplayName = nameof(ClearStorageInCommitErrorCase))]
    [Trait("Application", "UploadMedias - Use Cases")]
    public async Task ClearStorageInCommitErrorCase()
    {
        var video = _fixture.GetValidVideo();
        var validInput = _fixture.GetValidInput(video.Id);
        var videoFileName = StorageName.Create(video.Id, nameof(video.Media),
            validInput.VideoInput!.Extension);
        var trailerFileName = StorageName.Create(video.Id, nameof(video.Trailer),
            validInput.TrailerInput!.Extension);
        var videoStoragePath = $"storage/{videoFileName}";
        var trailerStoragePath = $"storage/{trailerFileName}";
        var fileNames = new List<string> { videoFileName, trailerFileName };
        var filePathNames = new List<string> { videoStoragePath, trailerStoragePath };
        _videoRepositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == video.Id),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(video);
        _StorageServiceMock.Setup(x => x.Upload(
            It.Is<string>(x => x == videoFileName),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(videoStoragePath);
        _StorageServiceMock.Setup(x => x.Upload(
            It.Is<string>(x => x == trailerFileName),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(trailerStoragePath);
        _UnitOfWorkMock.Setup(x => x.Commit(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong with commit"));

        var action = () => _useCase.Handle(validInput, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Something went wrong with commit");

        _videoRepositoryMock.VerifyAll();
        _StorageServiceMock.Verify(x => x.Upload(
            It.Is<string>(x => fileNames.Contains(x)),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()),
            Times.Exactly(2));
        _StorageServiceMock.Verify(x => x.Delete(
            It.Is<string>(fileName => filePathNames.Contains(fileName)),
            It.IsAny<CancellationToken>()), Times.Exactly(2));

    }

    [Fact(DisplayName = nameof(ClearOnlyOneFileStorageInCommitErrorCaseIfProvidedOnlyOneFile))]
    [Trait("Application", "UploadMedias - Use Cases")]
    public async Task ClearOnlyOneFileStorageInCommitErrorCaseIfProvidedOnlyOneFile()
    {
        var video = _fixture.GetValidVideo();
        video.UpdateTrailer(_fixture.GetValidMediaPath());
        video.UpdateMedia(_fixture.GetValidMediaPath());
        var validInput = _fixture.GetValidInput(video.Id, withTrailerFile: false);
        var videoFileName = StorageName.Create(video.Id, nameof(video.Media),
            validInput.VideoInput!.Extension);
        var videoStoragePath = $"storage/{videoFileName}";
        
        _videoRepositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == video.Id),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(video);
        _StorageServiceMock.Setup(x => x.Upload(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid().ToString());
        _StorageServiceMock.Setup(x => x.Upload(
            It.Is<string>(x => x == videoFileName),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(videoStoragePath);
        _UnitOfWorkMock.Setup(x => x.Commit(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong with commit"));

        var action = () => _useCase.Handle(validInput, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Something went wrong with commit");

        _videoRepositoryMock.VerifyAll();
        _StorageServiceMock.Verify(x => x.Upload(
            It.Is<string>(x => x == videoFileName),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()),
            Times.Exactly(1));
        _StorageServiceMock.Verify(x => x.Delete(
            It.Is<string>(fileName => fileName == videoStoragePath),
            It.IsAny<CancellationToken>()), Times.Exactly(1));
        _StorageServiceMock.Verify(x => x.Delete(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact(DisplayName = nameof(ThrowsWhenVideoNotFound))]
    [Trait("Application", "UploadMedias - Use Cases")]
    public async Task ThrowsWhenVideoNotFound()
    {
        var video = _fixture.GetValidVideo();
        var validInput = _fixture.GetValidInput(video.Id);

        _videoRepositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == video.Id),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Video not found"));

        var action = () => _useCase.Handle(validInput, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Video not found");
    }
}

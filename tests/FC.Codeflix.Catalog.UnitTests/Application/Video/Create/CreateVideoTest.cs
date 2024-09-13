using Moq;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using FC.Codeflix.Catalog.Domain.Extensions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.Create;
[Collection(nameof(CreateVideoTestFixture))]
public class CreateVideoTest(CreateVideoTestFixture fixture)
{

    [Fact(DisplayName = nameof(CreateVideo))]
    [Trait("Application", "CreateVideo - Use Cases")]
    private async Task CreateVideo()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCase.CreateVideo(repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>());
        var input = fixture.CreateValidCreateVideoInput();
        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x =>
            x.Insert(It.Is<DomainEntity.Video>(
                    video => video.Title == input.Title
                    && video.Description == input.Description
                    && video.Opened == input.Opened
                    && video.Published == input.Published
                    && video.Duration == input.Duration
                    && video.Rating == input.Rating
                    && video.Id != Guid.Empty
                    && video.YearLaunched == input.YearLaunched
                ),
                It.IsAny<CancellationToken>())
            );
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Should().NotBeNull();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.Id.Should().NotBe(Guid.Empty);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().NotBe(default);
    }

    [Fact(DisplayName = nameof(CreateVideoWithThumb))]
    [Trait("Application", "CreateVideo - Use Cases")]
    private async Task CreateVideoWithThumb()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedThumbName = $"thumb.jpg";
        storageServiceMock.Setup(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedThumbName);
        var useCase = new UseCase.CreateVideo(repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            storageServiceMock.Object);
        var input = fixture.CreateValidCreateVideoInput(thumb: fixture.GetValidImageFileInput());
        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x =>
            x.Insert(It.Is<DomainEntity.Video>(
                    video => video.Title == input.Title
                    && video.Description == input.Description
                    && video.Opened == input.Opened
                    && video.Published == input.Published
                    && video.Duration == input.Duration
                    && video.Rating == input.Rating
                    && video.Id != Guid.Empty
                    && video.YearLaunched == input.YearLaunched
                ),
                It.IsAny<CancellationToken>())
            );
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Should().NotBeNull();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.Id.Should().NotBe(Guid.Empty);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().NotBe(default);
        output.ThumbFileUrl.Should().Be(expectedThumbName);
    }

    [Fact(DisplayName = nameof(CreateVideoWithBanner))]
    [Trait("Application", "CreateVideo - Use Cases")]
    private async Task CreateVideoWithBanner()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedBannerName = $"banner.jpg";
        storageServiceMock.Setup(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBannerName);
        var useCase = new UseCase.CreateVideo(repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            storageServiceMock.Object);
        var input = fixture.CreateValidCreateVideoInput(banner: fixture.GetValidImageFileInput());
        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x =>
            x.Insert(It.Is<DomainEntity.Video>(
                    video => video.Title == input.Title
                    && video.Description == input.Description
                    && video.Opened == input.Opened
                    && video.Published == input.Published
                    && video.Duration == input.Duration
                    && video.Rating == input.Rating
                    && video.Id != Guid.Empty
                    && video.YearLaunched == input.YearLaunched
                ),
                It.IsAny<CancellationToken>())
            );
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Should().NotBeNull();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.Id.Should().NotBe(Guid.Empty);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().NotBe(default);
        output.ThumbFileUrl.Should().BeNull();
        output.BannerFileUrl.Should().Be(expectedBannerName);
    }

    [Fact(DisplayName = nameof(CreateVideoWithThumbHalf))]
    [Trait("Application", "CreateVideo - Use Cases")]
    private async Task CreateVideoWithThumbHalf()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedThumbHalfName = $"thumb-half.jpg";
        storageServiceMock.Setup(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedThumbHalfName);
        var useCase = new UseCase.CreateVideo(repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            storageServiceMock.Object);
        var input = fixture.CreateValidCreateVideoInput(thumbHalf: fixture.GetValidImageFileInput());
        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x =>
            x.Insert(It.Is<DomainEntity.Video>(
                    video => video.Title == input.Title
                    && video.Description == input.Description
                    && video.Opened == input.Opened
                    && video.Published == input.Published
                    && video.Duration == input.Duration
                    && video.Rating == input.Rating
                    && video.Id != Guid.Empty
                    && video.YearLaunched == input.YearLaunched
                ),
                It.IsAny<CancellationToken>())
            );
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Should().NotBeNull();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.Id.Should().NotBe(Guid.Empty);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().NotBe(default);
        output.ThumbFileUrl.Should().BeNull();
        output.ThumbHalfFileUrl.Should().Be(expectedThumbHalfName);
    }

    [Fact(DisplayName = nameof(CreateVideoWithAllImagesAndAllRelations))]
    [Trait("Application", "CreateVideo - Use Cases")]
    private async Task CreateVideoWithAllImagesAndAllRelations()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var exampleIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        categoryRepositoryMock.Setup(x =>
            x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds.AsReadOnly());
        var genreRepositoryMock = new Mock<IGenreRepository>();
        genreRepositoryMock.Setup(x =>
            x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds.AsReadOnly());
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        castMemberRepositoryMock.Setup(x =>
            x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds.AsReadOnly());
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedBannerName = $"banner.jpg";
        storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(y => y.EndsWith("/banner.jpg")),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBannerName);
        var expectedThumbName = $"thumb.jpg";
        storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(y => y.EndsWith("/thumb.jpg")),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedThumbName);
        var expectedThumbHalfName = $"thumbhalf.jpg";
        storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(y => y.EndsWith("/thumbhalf.jpg")),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedThumbHalfName);
        var useCase = new UseCase.CreateVideo(repositoryMock.Object,
            categoryRepositoryMock.Object,
            genreRepositoryMock.Object,
            castMemberRepositoryMock.Object,
            unitOfWorkMock.Object,
            storageServiceMock.Object);
        var input = fixture.CreateValidInputAllData();
        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x =>
            x.Insert(It.Is<DomainEntity.Video>(
                    video => video.Title == input.Title
                    && video.Description == input.Description
                    && video.Opened == input.Opened
                    && video.Published == input.Published
                    && video.Duration == input.Duration
                    && video.Rating == input.Rating
                    && video.Id != Guid.Empty
                    && video.YearLaunched == input.YearLaunched
                ),
                It.IsAny<CancellationToken>())
            );
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        storageServiceMock.VerifyAll();
        output.Should().NotBeNull();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.Id.Should().NotBe(Guid.Empty);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().NotBe(default);
        output.BannerFileUrl.Should().Be(expectedBannerName);
        output.ThumbFileUrl.Should().Be(expectedThumbName);
        output.ThumbHalfFileUrl.Should().Be(expectedThumbHalfName);
    }

    [Fact(DisplayName = nameof(CreateVideoWithMedia))]
    [Trait("Application", "CreateVideo - Use Cases")]
    private async Task CreateVideoWithMedia()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedMediaName = $"/storage/{fixture.GetValidMediaPath()}";
        storageServiceMock.Setup(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMediaName);
        var useCase = new UseCase.CreateVideo(repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            storageServiceMock.Object);
        var input = fixture.CreateValidCreateVideoInput(
            media: fixture.GetValidMediaFileInput());

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x =>
            x.Insert(It.Is<DomainEntity.Video>(
                    video => video.Title == input.Title
                    && video.Description == input.Description
                    && video.Opened == input.Opened
                    && video.Published == input.Published
                    && video.Duration == input.Duration
                    && video.Rating == input.Rating
                    && video.Id != Guid.Empty
                    && video.YearLaunched == input.YearLaunched
                ),
                It.IsAny<CancellationToken>())
            );
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Should().NotBeNull();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.Id.Should().NotBe(Guid.Empty);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().NotBe(default);
        output.VideoFileUrl.Should().Be(expectedMediaName);
    }

    [Fact(DisplayName = nameof(CreateVideoWithTrailer))]
    [Trait("Application", "CreateVideo - Use Cases")]
    private async Task CreateVideoWithTrailer()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedTrailerName = $"/storage/{fixture.GetValidMediaPath()}";
        storageServiceMock.Setup(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTrailerName);
        var useCase = new UseCase.CreateVideo(repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            storageServiceMock.Object);
        var input = fixture.CreateValidCreateVideoInput(
            trailer: fixture.GetValidMediaFileInput());

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x =>
            x.Insert(It.Is<DomainEntity.Video>(
                    video => video.Title == input.Title
                    && video.Description == input.Description
                    && video.Opened == input.Opened
                    && video.Published == input.Published
                    && video.Duration == input.Duration
                    && video.Rating == input.Rating
                    && video.Id != Guid.Empty
                    && video.YearLaunched == input.YearLaunched
                ),
                It.IsAny<CancellationToken>())
            );
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Should().NotBeNull();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.Id.Should().NotBe(Guid.Empty);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().NotBe(default);
        output.TrailerFileUrl.Should().Be(expectedTrailerName);
    }

    [Fact(DisplayName = nameof(ThrowsExceptionInUploadErrorCases))]
    [Trait("Application", "CreateVideo - Use Cases")]
    private async Task ThrowsExceptionInUploadErrorCases()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var exampleIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        categoryRepositoryMock.Setup(x =>
            x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds.AsReadOnly());
        var genreRepositoryMock = new Mock<IGenreRepository>();
        genreRepositoryMock.Setup(x =>
            x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds.AsReadOnly());
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        castMemberRepositoryMock.Setup(x =>
            x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds.AsReadOnly());
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedBannerName = $"banner.jpg";
        storageServiceMock.Setup(x => x.Upload(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong in Upload"));

        var useCase = new UseCase.CreateVideo(repositoryMock.Object,
            categoryRepositoryMock.Object,
            genreRepositoryMock.Object,
            castMemberRepositoryMock.Object,
            unitOfWorkMock.Object,
            storageServiceMock.Object);
        var input = fixture.CreateValidInputAllData();
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        action.Should().NotBeNull();
        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Something went wrong in Upload");
    }

    [Fact(DisplayName = nameof(ThrowsExceptionAndRollbackUploadErrorCases))]
    [Trait("Application", "CreateVideo - Use Cases")]
    private async Task ThrowsExceptionAndRollbackUploadErrorCases()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var exampleIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        categoryRepositoryMock.Setup(x =>
            x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds.AsReadOnly());
        var genreRepositoryMock = new Mock<IGenreRepository>();
        genreRepositoryMock.Setup(x =>
            x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds.AsReadOnly());
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        castMemberRepositoryMock.Setup(x =>
            x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds.AsReadOnly());
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedThumbName = $"123/thumb.jpg";
        storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(y => y.EndsWith("/thumb.jpg")),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedThumbName);
        var expectedBannerName = $"123/banner.jpg";
        storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(y => y.EndsWith("/banner.jpg")),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBannerName);
        storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(y => y.EndsWith("/thumbhalf.jpg")),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong in Upload"));

        var useCase = new UseCase.CreateVideo(repositoryMock.Object,
            categoryRepositoryMock.Object,
            genreRepositoryMock.Object,
            castMemberRepositoryMock.Object,
            unitOfWorkMock.Object,
            storageServiceMock.Object);
        var input = fixture.CreateValidInputAllData();
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        action.Should().NotBeNull();
        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Something went wrong in Upload");
        storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(x => x.EndsWith("/thumb.jpg") || x.EndsWith("/banner.jpg")),
            It.IsAny<CancellationToken>()
            ), Times.Exactly(2));
    }

    [Fact(DisplayName = nameof(ThrowsExceptionAndRollbackMediaUploadInCommitErrorCases))]
    [Trait("Application", "CreateVideo - Use Cases")]
    private async Task ThrowsExceptionAndRollbackMediaUploadInCommitErrorCases()
    {
        var input = fixture.CreateValidInputAllMedias();
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedMediaName = $"123-media.mp4";
        storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(y => y.EndsWith("/media.mp4")),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMediaName);
        var expectedTrailerName = $"123-trailer.mp4";
        storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(y => y.EndsWith("/trailer.mp4")),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTrailerName);
        unitOfWorkMock.Setup(x => x.Commit(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong in Commit"));

        var useCase = new UseCase.CreateVideo(repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            storageServiceMock.Object);
        
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        action.Should().NotBeNull();
        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Something went wrong in Commit");
        storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(x => x == expectedTrailerName || x == expectedMediaName),
            It.IsAny<CancellationToken>()
            ), Times.Exactly(2));
    }

    [Theory(DisplayName = nameof(CreateVideoThrowsWithInvalidInput))]
    [Trait("Application", "CreateVideo - Use Cases")]
    [ClassData(typeof(CreateVideoTestDataGenerator))]
    private async Task CreateVideoThrowsWithInvalidInput(
        CreateVideoInput input,
        string expectedValidationError)
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCase.CreateVideo(repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>());

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        var exceptionAssertion = await action.Should().ThrowAsync<EntityValidationException>();
        exceptionAssertion.WithMessage("There are validation errors")
            .Which.Errors!.First().Message.Should()
            .Be(expectedValidationError);
        repositoryMock.Verify(x =>
            x.Insert(It.IsAny<DomainEntity.Video>(), It.IsAny<CancellationToken>()),
            Times.Never
            );
    }

    [Fact(DisplayName = nameof(CreateVideoWithCategories))]
    [Trait("Application", "CreateVideo - Use Cases")]
    private async Task CreateVideoWithCategories()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var exampleCategoriesIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategoriesIds);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCase.CreateVideo(repositoryMock.Object,
            categoryRepositoryMock.Object,
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>());
        var input = fixture.CreateValidCreateVideoInput(exampleCategoriesIds);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.Id.Should().NotBe(Guid.Empty);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().NotBe(default);
        output.Categories!.Select(dto => dto.Id).Should().BeEquivalentTo(exampleCategoriesIds);
        repositoryMock.Verify(x =>
            x.Insert(It.Is<DomainEntity.Video>(
                    video => video.Title == input.Title
                    && video.Description == input.Description
                    && video.Opened == input.Opened
                    && video.Published == input.Published
                    && video.Duration == input.Duration
                    && video.Rating == input.Rating
                    && video.Id != Guid.Empty
                    && video.YearLaunched == input.YearLaunched
                    && video.Categories.All(categoryId =>
                        exampleCategoriesIds.Contains(categoryId))
                ),
                It.IsAny<CancellationToken>())
            );
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Fact(DisplayName = nameof(ThrowsWhenCategoryIdInvalid))]
    [Trait("Application", "CreateVideo - Use Cases")]
    private async Task ThrowsWhenCategoryIdInvalid()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var exampleCategoriesIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var removedCategoryId = exampleCategoriesIds[2];
        categoryRepositoryMock.Setup(x =>
            x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategoriesIds.FindAll(x => x != removedCategoryId).AsReadOnly());
        var useCase = new UseCase.CreateVideo(repositoryMock.Object,
            categoryRepositoryMock.Object,
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>());
        var input = fixture.CreateValidCreateVideoInput(exampleCategoriesIds);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {removedCategoryId}.");
        categoryRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(CreateVideoWithGenresIds))]
    [Trait("Application", "CreateVideo - Use Cases")]
    private async Task CreateVideoWithGenresIds()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var examplesIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        genreRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesIds);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCase.CreateVideo(repositoryMock.Object,
            categoryRepositoryMock.Object,
            genreRepositoryMock.Object,
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>());
        var input = fixture.CreateValidCreateVideoInput(genresIds: examplesIds);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.Id.Should().NotBe(Guid.Empty);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().NotBe(default);
        output.Categories.Should().BeEmpty();
        output.Genres!.Select(dto => dto.Id).Should().BeEquivalentTo(examplesIds);
        repositoryMock.Verify(x =>
            x.Insert(It.Is<DomainEntity.Video>(
                    video => video.Title == input.Title
                    && video.Description == input.Description
                    && video.Opened == input.Opened
                    && video.Published == input.Published
                    && video.Duration == input.Duration
                    && video.Rating == input.Rating
                    && video.Id != Guid.Empty
                    && video.YearLaunched == input.YearLaunched
                    && video.Genres.All(id => examplesIds.Contains(id))
                ),
                It.IsAny<CancellationToken>())
            );
        genreRepositoryMock.VerifyAll();
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Fact(DisplayName = nameof(ThrowsWhenInvalidGenreId))]
    [Trait("Application", "CreateVideo - Use Cases")]
    private async Task ThrowsWhenInvalidGenreId()
    {
        var examplesIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var removedId = examplesIds[2];
        var repositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        genreRepositoryMock.Setup(x =>
            x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesIds.FindAll(x => x != removedId).AsReadOnly());
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCase.CreateVideo(repositoryMock.Object,
            categoryRepositoryMock.Object,
            genreRepositoryMock.Object,
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>());
        var input = fixture.CreateValidCreateVideoInput(genresIds: examplesIds);

        var action = () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related genre id (or ids) not found: {removedId}.");
        genreRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(CreateVideoWithCastMembersIds))]
    [Trait("Application", "CreateVideo - Use Cases")]
    private async Task CreateVideoWithCastMembersIds()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var examplesIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        castMemberRepositoryMock.Setup(x =>
            x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesIds);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCase.CreateVideo(repositoryMock.Object,
            categoryRepositoryMock.Object,
            genreRepositoryMock.Object,
            castMemberRepositoryMock.Object,
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>());
        var input = fixture.CreateValidCreateVideoInput(castMembersIds: examplesIds);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.Id.Should().NotBe(Guid.Empty);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().NotBe(default);
        output.Categories!.Select(dto => dto.Id).Should().BeEmpty();
        output.Genres!.Select(dto => dto.Id).Should().BeEmpty();
        output.CastMembers!.Select(dto => dto.Id).Should().BeEquivalentTo(examplesIds);
        repositoryMock.Verify(x =>
            x.Insert(It.Is<DomainEntity.Video>(
                    video => video.Title == input.Title
                    && video.Description == input.Description
                    && video.Opened == input.Opened
                    && video.Published == input.Published
                    && video.Duration == input.Duration
                    && video.Rating == input.Rating
                    && video.Id != Guid.Empty
                    && video.YearLaunched == input.YearLaunched
                    && video.CastMembers.All(id => examplesIds.Contains(id))
                ),
                It.IsAny<CancellationToken>())
            );
        genreRepositoryMock.VerifyAll();
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Fact(DisplayName = nameof(ThrowsWhenInvalidCastMemberId))]
    [Trait("Application", "CreateVideo - Use Cases")]
    private async Task ThrowsWhenInvalidCastMemberId()
    {
        var examplesIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var removedId = examplesIds[2];
        var repositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        castMemberRepositoryMock.Setup(x =>
            x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesIds.FindAll(x => x != removedId).AsReadOnly());
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCase.CreateVideo(repositoryMock.Object,
            categoryRepositoryMock.Object,
            genreRepositoryMock.Object,
            castMemberRepositoryMock.Object,
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>());
        var input = fixture.CreateValidCreateVideoInput(castMembersIds: examplesIds);

        var action = () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related castMember id (or ids) not found: {removedId}.");
        castMemberRepositoryMock.VerifyAll();
    }


}

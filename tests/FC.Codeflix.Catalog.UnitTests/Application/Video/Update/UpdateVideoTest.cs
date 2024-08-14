using Moq;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.Extensions;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.Exceptions;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;
using FC.Codeflix.Catalog.Application.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.Update;
[Collection(nameof(UpdateVideoTestFixture))]
public class UpdateVideoTest
{
    private readonly UpdateVideoTestFixture _fixture;
    private readonly Mock<IVideoRepository> _videoRepositoryMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<ICastMemberRepository> _castMemberRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IStorageService> _storageServiceMock;
    private readonly UseCase.UpdateVideo _useCase;
    public UpdateVideoTest(UpdateVideoTestFixture fixture)
    {
        _fixture = fixture;
        _videoRepositoryMock = new();
        _unitOfWorkMock = new();
        _genreRepositoryMock = new();
        _categoryRepositoryMock = new();
        _castMemberRepositoryMock = new();
        _storageServiceMock = new();
        _useCase = new (_videoRepositoryMock.Object,
            _genreRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _castMemberRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _storageServiceMock.Object);
    }

    [Fact(DisplayName = nameof(UpdateVideoBasicInfo))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoBasicInfo()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.CreateValidInput(exampleVideo.Id);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(repo => repo.Update(
                It.Is<DomainEntity.Video>(video => 
                    video.Id == exampleVideo.Id &&
                    video.Title == input.Title &&
                    video.Description == input.Description && 
                    video.Rating == input.Rating &&
                    video.YearLaunched == input.YearLaunched &&
                    video.Opened == input.Opened &&
                    video.Published == input.Published &&
                    video.Duration == input.Duration), 
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
    }

    [Fact(DisplayName = nameof(UpdateVideoWithGenreIds))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoWithGenreIds()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var examplesGenresIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id, examplesGenresIds);

        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _genreRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(idsList => idsList.Count == examplesGenresIds.Count &&
                idsList.All(id => examplesGenresIds.Contains(id))),
            It.IsAny<CancellationToken>()))
           .ReturnsAsync(examplesGenresIds);

        var output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _genreRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(repo => repo.Update(
                It.Is<DomainEntity.Video>(video =>
                    video.Id == exampleVideo.Id &&
                    video.Title == input.Title &&
                    video.Description == input.Description &&
                    video.Rating == input.Rating &&
                    video.YearLaunched == input.YearLaunched &&
                    video.Opened == input.Opened &&
                    video.Published == input.Published &&
                    video.Duration == input.Duration &&
                    video.Genres.All(genreId => examplesGenresIds.Contains(genreId)) &&
                    video.Genres.Count == examplesGenresIds.Count),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.Genres!.Select(genre => genre.Id).ToList().Should()
            .BeEquivalentTo(examplesGenresIds);
    }

    [Fact(DisplayName = nameof(UpdateVideoThrowsWhenInvalidGenreId))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoThrowsWhenInvalidGenreId()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var examplesGenresIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var invalidGenreId = Guid.NewGuid();
        var inputInvalidIdsList = examplesGenresIds
            .Concat(new List<Guid> { invalidGenreId });
        var input = _fixture.CreateValidInput(exampleVideo.Id, genreIds: inputInvalidIdsList.ToList());

        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _genreRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
           .ReturnsAsync(examplesGenresIds);

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related genre id (or ids) not found: {invalidGenreId}.");
        _videoRepositoryMock.VerifyAll();
        _genreRepositoryMock.VerifyAll();
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
        
    }

    [Fact(DisplayName = nameof(UpdateVideoWithCategoryIds))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoWithCategoryIds()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var examplesIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id, categoryIds: examplesIds);

        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(idsList => idsList.Count == examplesIds.Count &&
                idsList.All(id => examplesIds.Contains(id))),
            It.IsAny<CancellationToken>()))
           .ReturnsAsync(examplesIds);

        var output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _categoryRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(repo => repo.Update(
                It.Is<DomainEntity.Video>(video =>
                    video.Id == exampleVideo.Id &&
                    video.Title == input.Title &&
                    video.Description == input.Description &&
                    video.Rating == input.Rating &&
                    video.YearLaunched == input.YearLaunched &&
                    video.Opened == input.Opened &&
                    video.Published == input.Published &&
                    video.Duration == input.Duration &&
                    video.Categories.All(categoryId => examplesIds.Contains(categoryId)) &&
                    video.Categories.Count == examplesIds.Count),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.Categories!.Select(category => category.Id).ToList().Should()
            .BeEquivalentTo(examplesIds);
    }

    [Fact(DisplayName = nameof(UpdateVideoThrowsWhenInvalidCategoryId))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoThrowsWhenInvalidCategoryId()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var examplesIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var invalidId = Guid.NewGuid();
        var inputInvalidIdsList = examplesIds
            .Concat(new List<Guid> { invalidId }).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id, categoryIds: inputInvalidIdsList);

        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
           .ReturnsAsync(examplesIds);

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {invalidId}.");
        _videoRepositoryMock.VerifyAll();
        _categoryRepositoryMock.VerifyAll();
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);

    }

    [Fact(DisplayName = nameof(UpdateVideoWithCastMemberIds))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoWithCastMemberIds()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var examplesIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id, castMemberIds: examplesIds);

        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(idsList => idsList.Count == examplesIds.Count &&
                idsList.All(id => examplesIds.Contains(id))),
            It.IsAny<CancellationToken>()))
           .ReturnsAsync(examplesIds);

        var output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _castMemberRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(repo => repo.Update(
                It.Is<DomainEntity.Video>(video =>
                    video.Id == exampleVideo.Id &&
                    video.Title == input.Title &&
                    video.Description == input.Description &&
                    video.Rating == input.Rating &&
                    video.YearLaunched == input.YearLaunched &&
                    video.Opened == input.Opened &&
                    video.Published == input.Published &&
                    video.Duration == input.Duration &&
                    video.CastMembers.All(id => examplesIds.Contains(id)) &&
                    video.CastMembers.Count == examplesIds.Count),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.CastMembers!.Select(category => category.Id).ToList().Should()
            .BeEquivalentTo(examplesIds);
    }

    [Fact(DisplayName = nameof(UpdateVideoThrowsWhenInvalidCastMemberId))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoThrowsWhenInvalidCastMemberId()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var examplesIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var invalidId = Guid.NewGuid();
        var inputInvalidIdsList = examplesIds
            .Concat(new List<Guid> { invalidId }).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id, castMemberIds: inputInvalidIdsList);

        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
           .ReturnsAsync(examplesIds);

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related castMember id (or ids) not found: {invalidId}.");
        _videoRepositoryMock.VerifyAll();
        _castMemberRepositoryMock.VerifyAll();
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);

    }

    [Fact(DisplayName = nameof(UpdateVideoWithoutRelationsWithRelations))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoWithoutRelationsWithRelations()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var examplesGenresIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var examplesCategoriesIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var examplesCastMembersIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id, examplesGenresIds, 
            examplesCategoriesIds, examplesCastMembersIds);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _genreRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(idsList => idsList.Count == examplesGenresIds.Count &&
                idsList.All(id => examplesGenresIds.Contains(id))),
            It.IsAny<CancellationToken>()))
           .ReturnsAsync(examplesGenresIds);
        _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(idsList => idsList.Count == examplesCategoriesIds.Count &&
                idsList.All(id => examplesCategoriesIds.Contains(id))),
            It.IsAny<CancellationToken>()))
           .ReturnsAsync(examplesCategoriesIds);
        _castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(idsList => idsList.Count == examplesCastMembersIds.Count &&
                idsList.All(id => examplesCastMembersIds.Contains(id))),
            It.IsAny<CancellationToken>()))
           .ReturnsAsync(examplesCastMembersIds);

        var output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _genreRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(repo => repo.Update(
                It.Is<DomainEntity.Video>(video =>
                    video.Id == exampleVideo.Id &&
                    video.Title == input.Title &&
                    video.Description == input.Description &&
                    video.Rating == input.Rating &&
                    video.YearLaunched == input.YearLaunched &&
                    video.Opened == input.Opened &&
                    video.Published == input.Published &&
                    video.Duration == input.Duration &&
                    video.Genres.All(genreId => examplesGenresIds.Contains(genreId)) &&
                    video.Genres.Count == examplesGenresIds.Count &&
                    video.Categories.All(id => examplesCategoriesIds.Contains(id)) &&
                    video.Categories.Count == examplesCategoriesIds.Count &&
                    video.CastMembers.All(id => examplesCastMembersIds.Contains(id)) &&
                    video.CastMembers.Count == examplesCastMembersIds.Count),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.Genres!.Select(genre => genre.Id).ToList().Should()
            .BeEquivalentTo(examplesGenresIds);
        output.Categories!.Select(category => category.Id).ToList().Should()
            .BeEquivalentTo(examplesCategoriesIds);
        output.CastMembers!.Select(cm => cm.Id).ToList().Should()
            .BeEquivalentTo(examplesCastMembersIds);
    }

    [Fact(DisplayName = nameof(UpdateVideoWithRelationsUpdatingRelations))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoWithRelationsUpdatingRelations()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var examplesGenresIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var examplesCategoriesIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var examplesCastMembersIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id, examplesGenresIds,
            examplesCategoriesIds, examplesCastMembersIds);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _genreRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(idsList => idsList.Count == examplesGenresIds.Count &&
                idsList.All(id => examplesGenresIds.Contains(id))),
            It.IsAny<CancellationToken>()))
           .ReturnsAsync(examplesGenresIds);
        _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(idsList => idsList.Count == examplesCategoriesIds.Count &&
                idsList.All(id => examplesCategoriesIds.Contains(id))),
            It.IsAny<CancellationToken>()))
           .ReturnsAsync(examplesCategoriesIds);
        _castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(idsList => idsList.Count == examplesCastMembersIds.Count &&
                idsList.All(id => examplesCastMembersIds.Contains(id))),
            It.IsAny<CancellationToken>()))
           .ReturnsAsync(examplesCastMembersIds);

        var output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _genreRepositoryMock.VerifyAll();
        _categoryRepositoryMock.VerifyAll();
        _castMemberRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(repo => repo.Update(
                It.Is<DomainEntity.Video>(video =>
                    video.Id == exampleVideo.Id &&
                    video.Title == input.Title &&
                    video.Description == input.Description &&
                    video.Rating == input.Rating &&
                    video.YearLaunched == input.YearLaunched &&
                    video.Opened == input.Opened &&
                    video.Published == input.Published &&
                    video.Duration == input.Duration &&
                    video.Genres.All(genreId => examplesGenresIds.Contains(genreId)) &&
                    video.Genres.Count == examplesGenresIds.Count &&
                    video.Categories.All(id => examplesCategoriesIds.Contains(id)) &&
                    video.Categories.Count == examplesCategoriesIds.Count &&
                    video.CastMembers.All(id => examplesCastMembersIds.Contains(id)) &&
                    video.CastMembers.Count == examplesCastMembersIds.Count),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.Genres!.Select(genre => genre.Id).ToList().Should()
            .BeEquivalentTo(examplesGenresIds);
        output.Categories!.Select(category => category.Id).ToList().Should()
            .BeEquivalentTo(examplesCategoriesIds);
        output.CastMembers!.Select(cm => cm.Id).ToList().Should()
            .BeEquivalentTo(examplesCastMembersIds);
    }

    [Fact(DisplayName = nameof(UpdateVideoWithRelationsRemovingRelations))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoWithRelationsRemovingRelations()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        
        var input = _fixture.CreateValidInput(exampleVideo.Id, new List<Guid>(),
            new List<Guid>(), new List<Guid>());
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.Genres!.Should().BeEmpty();
        output.Categories!.Should().BeEmpty();
        output.CastMembers!.Should().BeEmpty();
        _videoRepositoryMock.VerifyAll();
        _genreRepositoryMock.Verify(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()), Times.Never);
        _categoryRepositoryMock.Verify(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()), Times.Never);
        _castMemberRepositoryMock.Verify(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()), Times.Never);
        _videoRepositoryMock.Verify(repo => repo.Update(
                It.Is<DomainEntity.Video>(video =>
                    video.Id == exampleVideo.Id &&
                    video.Title == input.Title &&
                    video.Description == input.Description &&
                    video.Rating == input.Rating &&
                    video.YearLaunched == input.YearLaunched &&
                    video.Opened == input.Opened &&
                    video.Published == input.Published &&
                    video.Duration == input.Duration &&
                    video.Genres.Count == 0 &&
                    video.Categories.Count == 0 &&
                    video.CastMembers.Count == 0),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Fact(DisplayName = nameof(UpdateVideoWithRelationsKeepRelationWhenReceiveNull))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoWithRelationsKeepRelationWhenReceiveNull()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();

        var input = _fixture.CreateValidInput(exampleVideo.Id, null, null, null);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.Genres!.Select(genre => genre.Id).ToList().Should()
            .BeEquivalentTo(exampleVideo.Genres);
        output.Categories!.Select(category => category.Id).ToList().Should()
            .BeEquivalentTo(exampleVideo.Categories);
        output.CastMembers!.Select(cm => cm.Id).ToList().Should()
            .BeEquivalentTo(exampleVideo.CastMembers);
        _videoRepositoryMock.VerifyAll();
        _genreRepositoryMock.Verify(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()), Times.Never);
        _categoryRepositoryMock.Verify(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()), Times.Never);
        _castMemberRepositoryMock.Verify(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()), Times.Never);
        _videoRepositoryMock.Verify(repo => repo.Update(
                It.Is<DomainEntity.Video>(video =>
                    video.Id == exampleVideo.Id &&
                    video.Title == input.Title &&
                    video.Description == input.Description &&
                    video.Rating == input.Rating &&
                    video.YearLaunched == input.YearLaunched &&
                    video.Opened == input.Opened &&
                    video.Published == input.Published &&
                    video.Duration == input.Duration &&
                    video.Genres.Count == exampleVideo.Genres.Count &&
                    video.Categories.Count == exampleVideo.Categories.Count &&
                    video.CastMembers.Count == exampleVideo.CastMembers.Count),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Theory(DisplayName = nameof(UpdateVideosThrowsWhenReceiveInvalidInput))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    [ClassData(typeof(UpdateVideoTestDataGenerator))]
    public async Task UpdateVideosThrowsWhenReceiveInvalidInput(
        UpdateVideoInput input,
        string expectedValidationError)
    {
        var exampleVideo = _fixture.GetValidVideo();
        _videoRepositoryMock.Setup(x => x.Get(It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        
        var action = () => _useCase.Handle(input, CancellationToken.None);

        var exceptionAssertion = await action.Should().ThrowAsync<EntityValidationException>();
        exceptionAssertion.WithMessage("There are validation errors")
            .Which.Errors!.First().Message.Should()
            .Be(expectedValidationError);
        _videoRepositoryMock.VerifyAll();
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
        
    }

    [Fact(DisplayName = nameof(UpdateVideoThrowsWhenNotFound))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoThrowsWhenNotFound()
    {
        var input = _fixture.CreateValidInput(Guid.NewGuid());
        _videoRepositoryMock.Setup(x => x.Get(It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Video not found"));

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Video not found");
        _videoRepositoryMock.Verify(repo => repo.Update(
            It.IsAny<DomainEntity.Video>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Commit(
            It.IsAny<CancellationToken>()),Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideoWithBannerWhenVideoNotHaveBanner))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoWithBannerWhenVideoNotHaveBanner()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.CreateValidInput(exampleVideo.Id, 
            banner: _fixture.GetValidImageFileInput());
        var bannerPath = $"storage/banner-{input.Banner!.Extension}";
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(name => name == StorageName.Create(exampleVideo.Id, nameof(exampleVideo.Banner), input.Banner.Extension)),
            It.IsAny<MemoryStream>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(bannerPath);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.BannerFileUrl.Should().Be(bannerPath);
        _videoRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(repo => repo.Update(
                It.Is<DomainEntity.Video>(video =>
                    video.Id == exampleVideo.Id &&
                    video.Title == input.Title &&
                    video.Description == input.Description &&
                    video.Rating == input.Rating &&
                    video.YearLaunched == input.YearLaunched &&
                    video.Opened == input.Opened &&
                    video.Published == input.Published &&
                    video.Duration == input.Duration &&
                    video.Banner!.Path == bannerPath
                   ),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _storageServiceMock.VerifyAll();
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Fact(DisplayName = nameof(UpdateVideoKeepBannerWhenReceiveNull))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoKeepBannerWhenReceiveNull()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            banner: null);
        var bannerPath = $"storage/banner-{exampleVideo.Banner!.Path}";
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        
        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.BannerFileUrl.Should().Be(exampleVideo.Banner!.Path);
        _videoRepositoryMock.VerifyAll();
        _storageServiceMock.Verify(x => x.Upload(It.IsAny<string>(),
            It.IsAny<FileStream>(), It.IsAny<CancellationToken>()), Times.Never);
        _videoRepositoryMock.Verify(repo => repo.Update(
                It.Is<DomainEntity.Video>(video =>
                    video.Id == exampleVideo.Id &&
                    video.Title == input.Title &&
                    video.Description == input.Description &&
                    video.Rating == input.Rating &&
                    video.YearLaunched == input.YearLaunched &&
                    video.Opened == input.Opened &&
                    video.Published == input.Published &&
                    video.Duration == input.Duration &&
                    video.Banner!.Path == exampleVideo.Banner.Path
                   ),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Fact(DisplayName = nameof(UpdateVideoWithThumbWhenVideoNotHaveThumb))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoWithThumbWhenVideoNotHaveThumb()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            thumb: _fixture.GetValidImageFileInput());
        var path = $"storage/thumb-{input.Thumb!.Extension}";
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(name => name == StorageName.Create(exampleVideo.Id, nameof(exampleVideo.Thumb), input.Thumb.Extension)),
            It.IsAny<MemoryStream>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(path);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.ThumbFileUrl.Should().Be(path);
        _videoRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(repo => repo.Update(
                It.Is<DomainEntity.Video>(video =>
                    video.Id == exampleVideo.Id &&
                    video.Title == input.Title &&
                    video.Description == input.Description &&
                    video.Rating == input.Rating &&
                    video.YearLaunched == input.YearLaunched &&
                    video.Opened == input.Opened &&
                    video.Published == input.Published &&
                    video.Duration == input.Duration &&
                    video.Thumb!.Path == path
                   ),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _storageServiceMock.VerifyAll();
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Fact(DisplayName = nameof(UpdateVideoKeepThumbWhenReceiveNull))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoKeepThumbWhenReceiveNull()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            thumb: null);
        var path = $"storage/thumb-{exampleVideo.Thumb!.Path}";
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.ThumbFileUrl.Should().Be(exampleVideo.Thumb!.Path);
        _videoRepositoryMock.VerifyAll();
        _storageServiceMock.Verify(x => x.Upload(It.IsAny<string>(),
            It.IsAny<FileStream>(), It.IsAny<CancellationToken>()), Times.Never);
        _videoRepositoryMock.Verify(repo => repo.Update(
                It.Is<DomainEntity.Video>(video =>
                    video.Id == exampleVideo.Id &&
                    video.Title == input.Title &&
                    video.Description == input.Description &&
                    video.Rating == input.Rating &&
                    video.YearLaunched == input.YearLaunched &&
                    video.Opened == input.Opened &&
                    video.Published == input.Published &&
                    video.Duration == input.Duration &&
                    video.Thumb!.Path == exampleVideo.Thumb!.Path
                   ),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Fact(DisplayName = nameof(UpdateVideoWithThumbHalfWhenVideoNotHaveThumbHalf))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoWithThumbHalfWhenVideoNotHaveThumbHalf()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            thumbHalf: _fixture.GetValidImageFileInput());
        var path = $"storage/thumbhalf-{input.ThumbHalf!.Extension}";
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(name => name == StorageName.Create(exampleVideo.Id, nameof(exampleVideo.ThumbHalf), input.ThumbHalf.Extension)),
            It.IsAny<MemoryStream>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(path);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.ThumbHalfFileUrl.Should().Be(path);
        _videoRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(repo => repo.Update(
                It.Is<DomainEntity.Video>(video =>
                    video.Id == exampleVideo.Id &&
                    video.Title == input.Title &&
                    video.Description == input.Description &&
                    video.Rating == input.Rating &&
                    video.YearLaunched == input.YearLaunched &&
                    video.Opened == input.Opened &&
                    video.Published == input.Published &&
                    video.Duration == input.Duration &&
                    video.ThumbHalf!.Path == path
                   ),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _storageServiceMock.VerifyAll();
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Fact(DisplayName = nameof(UpdateVideoKeepThumbHalfWhenReceiveNull))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoKeepThumbHalfWhenReceiveNull()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            thumbHalf: null);
        var path = $"storage/thumbhalf-{exampleVideo.ThumbHalf!.Path}";
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.ThumbHalfFileUrl.Should().Be(exampleVideo.ThumbHalf!.Path);
        _videoRepositoryMock.VerifyAll();
        _storageServiceMock.Verify(x => x.Upload(It.IsAny<string>(),
            It.IsAny<FileStream>(), It.IsAny<CancellationToken>()), Times.Never);
        _videoRepositoryMock.Verify(repo => repo.Update(
                It.Is<DomainEntity.Video>(video =>
                    video.Id == exampleVideo.Id &&
                    video.Title == input.Title &&
                    video.Description == input.Description &&
                    video.Rating == input.Rating &&
                    video.YearLaunched == input.YearLaunched &&
                    video.Opened == input.Opened &&
                    video.Published == input.Published &&
                    video.Duration == input.Duration &&
                    video.ThumbHalf!.Path == exampleVideo.ThumbHalf!.Path
                   ),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }
}

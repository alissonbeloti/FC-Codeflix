using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using FluentAssertions;
using Moq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.Update;
[Collection(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTest
{
    private readonly UpdateGenreTestFixture _fixture;

    public UpdateGenreTest(UpdateGenreTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenre()
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenra();
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x =>
            x.Get(exampleGenre.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);
        var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            _fixture.GetCategoryRepositoryMock().Object);
        var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newNameExample, newIsActive);

        GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(newIsActive);
        output.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        output.Categories.Should().HaveCount(0);
        genreRepositoryMock.Verify(x => x.Get(exampleGenre.Id, It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.Verify(x => x.Update(
            It.Is<DomainEntity.Genre>(y => y.Id == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory(DisplayName = nameof(UpdateGenreOnlyName))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateGenreOnlyName(bool isActive)
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenra(isActive: isActive);
        var newNameExample = _fixture.GetValidGenreName();

        genreRepositoryMock.Setup(x =>
            x.Get(exampleGenre.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);
        var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            _fixture.GetCategoryRepositoryMock().Object);
        var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newNameExample);

        GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(isActive);
        output.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        output.Categories.Should().HaveCount(0);
        genreRepositoryMock.Verify(x => x.Get(exampleGenre.Id, It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.Verify(x => x.Update(
            It.Is<DomainEntity.Genre>(y => y.Id == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateGenreAddingCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreAddingCategoriesIds()
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenra();
        var exampleCategoriresIds = _fixture.GetRamdoGuids();
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x =>
            x.Get(exampleGenre.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategoriresIds);
        var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object);
        var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newNameExample, newIsActive,
            exampleCategoriresIds);

        GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(newIsActive);
        output.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        output.Categories.Should().HaveCount(exampleCategoriresIds.Count);
        exampleCategoriresIds.ForEach(
            expectedId => output.Categories.Should().Contain(expectedId)
            );
        genreRepositoryMock.Verify(x => x.Get(exampleGenre.Id, It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.Verify(x => x.Update(
            It.Is<DomainEntity.Genre>(y => y.Id == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateGenreReplacingCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreReplacingCategoriesIds()
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenra(categoriesIds: _fixture.GetRamdoGuids());
        var exampleCategoriresIds = _fixture.GetRamdoGuids();
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x =>
            x.Get(exampleGenre.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategoriresIds);
        var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object);
        var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newNameExample, newIsActive,
            exampleCategoriresIds);

        GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(newIsActive);
        output.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        output.Categories.Should().HaveCount(exampleCategoriresIds.Count);
        exampleCategoriresIds.ForEach(
            expectedId => output.Categories.Should().Contain(expectedId)
            );
        genreRepositoryMock.Verify(x => x.Get(exampleGenre.Id, It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.Verify(x => x.Update(
            It.Is<DomainEntity.Genre>(y => y.Id == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateGenreWithoutCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreWithoutCategoriesIds()
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleCategoriresIds = _fixture.GetRamdoGuids();
        var exampleGenre = _fixture.GetExampleGenra(categoriesIds: exampleCategoriresIds);
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x =>
            x.Get(exampleGenre.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);
        var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object);
        var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newNameExample, newIsActive);

        GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(newIsActive);
        output.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        output.Categories.Should().HaveCount(exampleCategoriresIds.Count);
        exampleCategoriresIds.ForEach(
            expectedId => output.Categories.Should().Contain(expectedId)
            );
        genreRepositoryMock.Verify(x => x.Get(exampleGenre.Id, It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.Verify(x => x.Update(
            It.Is<DomainEntity.Genre>(y => y.Id == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateGenreWithEmptyCategoriesIdsList))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreWithEmptyCategoriesIdsList()
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleCategoriresIds = _fixture.GetRamdoGuids();
        var exampleGenre = _fixture.GetExampleGenra(categoriesIds: exampleCategoriresIds);
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x =>
            x.Get(exampleGenre.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);
        var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object);
        var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newNameExample, newIsActive,
            new List<Guid>());

        GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(newIsActive);
        output.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        output.Categories.Should().HaveCount(0);
        genreRepositoryMock.Verify(x => x.Get(exampleGenre.Id, It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.Verify(x => x.Update(
            It.Is<DomainEntity.Genre>(y => y.Id == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task ThrowWhenCategoryNotFound()
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenra(categoriesIds: _fixture.GetRamdoGuids());
        var exampleNewCategoriesIds = _fixture.GetRamdoGuids(10);
        var listReturnedByCategoryReopository = exampleNewCategoriesIds.
            GetRange(0, exampleNewCategoriesIds.Count - 2);
        var IdsNotReturnedByCategoryRepository = exampleNewCategoriesIds.
            GetRange(exampleNewCategoriesIds.Count - 2, 2);
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x =>
            x.Get(exampleGenre.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(listReturnedByCategoryReopository);
        var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object);
        var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newNameExample, newIsActive,
            exampleNewCategoriesIds);
        var notFoundIdsAsString = string.Join(", ", IdsNotReturnedByCategoryRepository);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {notFoundIdsAsString}.");

        
    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task ThrowWhenNotFound()
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var idExample = Guid.NewGuid();
        genreRepositoryMock.Setup(x =>
            x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Genre '{idExample}' not found."));
        var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object,
            _fixture.GetUnitOfWorkMock().Object,
            _fixture.GetCategoryRepositoryMock().Object);
        var input = new UseCase.UpdateGenreInput(idExample, _fixture.GetValidGenreName(), true);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        action.Should().NotBeNull();
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{idExample}' not found.");
    }

    [Theory(DisplayName = nameof(ThrowWhenNameIsInvalid))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task ThrowWhenNameIsInvalid(string? name)
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenra();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x =>
            x.Get(exampleGenre.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);
        var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            _fixture.GetCategoryRepositoryMock().Object);
        var input = new UseCase.UpdateGenreInput(exampleGenre.Id, name!, newIsActive);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        action.Should().NotBeNull();
        await action.Should().ThrowAsync<EntityValidationException>()
            .WithMessage($"Name should not be empty or null");
    }
}

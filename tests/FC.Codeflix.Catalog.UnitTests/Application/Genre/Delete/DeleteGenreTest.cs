using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;

using Moq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.Delete;
[Collection(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTest
{
    private readonly DeleteGenreTestFixture _fixture;

    public DeleteGenreTest(DeleteGenreTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("Application", "DeleteGenre - Use Cases")]
    public async Task DeleteGenre()
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenra();
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x =>
            x.Get(exampleGenre.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);
        var useCase = new UseCase.DeleteGenre(genreRepositoryMock.Object,
            unitOfWorkMock.Object);
        var input = new UseCase.DeleteGenreInput(exampleGenre.Id);

        await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(x => x.Get(exampleGenre.Id, 
            It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.Verify(x => x.Delete(
            It.Is<DomainEntity.Genre>(y => y.Id == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowWenNotFound))]
    [Trait("Application", "DeleteGenre - Use Cases")]
    public async Task ThrowWenNotFound()
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleId = Guid.NewGuid();
        genreRepositoryMock.Setup(x =>
            x.Get(It.Is<Guid>(x => x == exampleId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Genre '{exampleId}' not found"));
        var useCase = new UseCase.DeleteGenre(genreRepositoryMock.Object, unitOfWorkMock.Object);
        var input = new UseCase.DeleteGenreInput(exampleId);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{exampleId}' not found");
        genreRepositoryMock.Verify(x => x.Get(It.Is<Guid>(x => x == exampleId),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}

using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre;
using Moq;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.Exceptions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.Get;
[Collection(nameof(GetGrenreTestFixture))]
public class GetGrenreTest
{
    private readonly GetGrenreTestFixture _fixture;

    public GetGrenreTest(GetGrenreTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("Application", "GetGenre - Use Cases")]
    public async Task GetGenre()
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var exampleGenre = _fixture.GetExampleGenra(categoriesIds: _fixture.GetRamdoGuids());
        genreRepositoryMock.Setup(x =>
            x.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);
        var useCase = new UseCase.GetGenre(genreRepositoryMock.Object);
        var input = new UseCase.GetGenreInput(exampleGenre.Id);

        GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(exampleGenre.Name);
        output.IsActive.Should().Be(exampleGenre.IsActive);
        output.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        output.Categories.Should().HaveCount(exampleGenre.Categories.Count);
        foreach (var category in exampleGenre.Categories)
        {
            output.Categories.Should().Contain(category);
        }
        genreRepositoryMock.Verify(x => x.Get(It.Is<Guid>(x => x == exampleGenre.Id), 
            It.IsAny<CancellationToken>()), Times.Once);
 
    }

    [Fact(DisplayName = nameof(ThrowWenNotFound))]
    [Trait("Application", "GetGenre - Use Cases")]
    public async Task ThrowWenNotFound()
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var exampleId = Guid.NewGuid();
        genreRepositoryMock.Setup(x =>
            x.Get(It.Is<Guid>(x => x == exampleId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Genre '{exampleId}' not found"));
        var useCase = new UseCase.GetGenre(genreRepositoryMock.Object);
        var input = new UseCase.GetGenreInput(exampleId);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{exampleId}' not found");
        genreRepositoryMock.Verify(x => x.Get(It.Is<Guid>(x => x == exampleId),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}

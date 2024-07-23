using Moq;
using FluentAssertions;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenres;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.List;


[Collection(nameof(ListGenreTestFixture))]
public class ListGenreTest
{
    public readonly ListGenreTestFixture _fixture;

    public ListGenreTest(ListGenreTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(ListGenre))]
    [Trait("Application", "ListGenre - Use Cases")]
    public async Task ListGenre()
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var exampleGenresList = _fixture.GetExampleGenresList();
        var input = _fixture.GetExampleInput();

        var outputRepositorySearch = new SearchOutput<DomainEntity.Genre>(
                currentPage: input.Page,
                perPage: input.PerPage,
                items: (IReadOnlyList<DomainEntity.Genre>)exampleGenresList,
                total: new Random().Next(50, 200)
                );

        genreRepositoryMock.Setup(x => x.SearchAsync(
            It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page &&
                searchInput.PerPage == input.PerPage &&
                searchInput.Search == input.Search &&
                searchInput.OrderBy == input.Sort &&
                searchInput.Order == input.Dir
                ),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(outputRepositorySearch);


        var useCase = new UseCase.ListGenres(genreRepositoryMock.Object);
        
        UseCase.ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        foreach (var outputItem in output.Items)
        {
            var repositoryGenre = outputRepositorySearch.Items
                .FirstOrDefault(x => x.Id == outputItem.Id);
            repositoryGenre.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(repositoryGenre!.Name);
            outputItem.IsActive.Should().Be(repositoryGenre.IsActive);
            outputItem.CreatedAt.Should().Be(repositoryGenre.CreatedAt);
            outputItem.Categories.Should().HaveCount(repositoryGenre.Categories.Count);
            foreach (var expectedId in repositoryGenre.Categories)
                outputItem.Categories.Should().Contain(expectedId);
        }
        genreRepositoryMock.Verify(x => x.SearchAsync(It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page &&
                searchInput.PerPage == input.PerPage &&
                searchInput.Search == input.Search &&
                searchInput.OrderBy == input.Sort &&
                searchInput.Order == input.Dir),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ListEmpty))]
    [Trait("Application", "ListGenre - Use Cases")]
    public async Task ListEmpty()
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();
        var input = _fixture.GetExampleInput();

        var outputRepositorySearch = new SearchOutput<DomainEntity.Genre>(
                currentPage: input.Page,
                perPage: input.PerPage,
                items: (IReadOnlyList<DomainEntity.Genre>)new List<DomainEntity.Genre>(),
                total: new Random().Next(50, 200)
                );

        genreRepositoryMock.Setup(x => x.SearchAsync(
            It.IsAny<SearchInput>(),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(outputRepositorySearch);

        var useCase = new UseCase.ListGenres(genreRepositoryMock.Object);

        UseCase.ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        
        genreRepositoryMock.Verify(x => x.SearchAsync(It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page &&
                searchInput.PerPage == input.PerPage &&
                searchInput.Search == input.Search &&
                searchInput.OrderBy == input.Sort &&
                searchInput.Order == input.Dir),
            It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact(DisplayName = nameof(ListUsingDefaultInputValues))]
    [Trait("Application", "ListGenre - Use Cases")]
    public async Task ListUsingDefaultInputValues()
    {
        var genreRepositoryMock = _fixture.GetRepositoryMock();

        var outputRepositorySearch = new SearchOutput<DomainEntity.Genre>(
                currentPage: 1,
                perPage: 15,
                items: (IReadOnlyList<DomainEntity.Genre>)new List<DomainEntity.Genre>(),
                total: 0
                );

        genreRepositoryMock.Setup(x => x.SearchAsync(
            It.IsAny<SearchInput>(),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(outputRepositorySearch);

        var useCase = new UseCase.ListGenres(genreRepositoryMock.Object);

        UseCase.ListGenresOutput output = await useCase.Handle(new UseCase.ListGenresInput(), CancellationToken.None);

        
        genreRepositoryMock.Verify(x => x.SearchAsync(It.Is<SearchInput>(searchInput =>
                searchInput.Page == 1 &&
                searchInput.PerPage == 15 &&
                searchInput.Search == "" &&
                searchInput.OrderBy == "" &&
                searchInput.Order == SearchOrder.Asc),
            It.IsAny<CancellationToken>()), Times.Once);

    }
}

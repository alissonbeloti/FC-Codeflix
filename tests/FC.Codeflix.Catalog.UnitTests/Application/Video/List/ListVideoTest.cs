using Moq;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Application.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.ListVideo;
using FC.Codeflix.Catalog.Domain.Extensions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.List;
[Collection(nameof(ListVideoTestFixture))]
public class ListVideoTest
{
    private readonly ListVideoTestFixture _fixture;
    private readonly Mock<IVideoRepository> _videoRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<ICastMemberRepository> _castMemberRepositoryMock;
    private readonly UseCase.ListVideos _useCase;
    public ListVideoTest(ListVideoTestFixture fixture)
    {
        _fixture = fixture;
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _genreRepositoryMock = new Mock<IGenreRepository>();
        _castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        _useCase = new UseCase.ListVideos(_videoRepositoryMock.Object, 
            _categoryRepositoryMock.Object, _genreRepositoryMock.Object,
            _castMemberRepositoryMock.Object);
    }

    [Fact(DisplayName = nameof(ListVideos))]
    [Trait("Application", "LIstVideos - Use Cases")]
    public async Task ListVideos()
    {
        var exampleVideoList = _fixture.CreateExampleVideosList();
        var input = new UseCase.ListVideosInput(1, 10, "", "", SearchOrder.Asc);
        var outputRepositorySearch = new SearchOutput<DomainEntity.Video>(
                currentPage: input.Page,
                perPage: input.PerPage,
                items: exampleVideoList,
                total: exampleVideoList.Count
                );
        _videoRepositoryMock.Setup(x => x.SearchAsync(
            It.Is<SearchInput>(s => s.Page == input.Page &&
                s.PerPage == input.PerPage &&
                s.Search == input.Search &&
                s.OrderBy == input.Sort &&
                s.Order == input.Dir
                ),
            It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(outputRepositorySearch);

        PaginatorListOutput<VideoModelOutput> output = await _useCase.Handle(input, CancellationToken.None);

        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleVideoList.Count);
        output.Items.Should().HaveCount(exampleVideoList.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleVideoList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Title.Should().Be(exampleItem!.Title);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.Opened.Should().Be(exampleItem.Opened);
            outputItem.Published.Should().Be(exampleItem.Published);
            outputItem.Duration.Should().Be(exampleItem.Duration);
            outputItem.Rating.Should().Be(exampleItem.Rating.ToStringSignal());
            outputItem.YearLaunched.Should().Be(exampleItem.YearLaunched);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
            outputItem.ThumbFileUrl.Should().Be(exampleItem.Thumb!.Path);
            outputItem.ThumbHalfFileUrl.Should().Be(exampleItem.ThumbHalf!.Path);
            outputItem.BannerFileUrl.Should().Be(exampleItem.Banner!.Path);
            outputItem.VideoFileUrl.Should().Be(exampleItem.Media!.FilePath);
            outputItem.TrailerFileUrl.Should().Be(exampleItem.Trailer!.FilePath);
            outputItem.Categories!.Select(dto => dto.Id).Should().BeEquivalentTo(exampleItem.Categories);
            outputItem.CastMembers!.Select(dto => dto.Id).Should().BeEquivalentTo(exampleItem.CastMembers);
            outputItem.Genres!.Select(dto => dto.Id).Should().BeEquivalentTo(exampleItem.Genres);
        });
        _videoRepositoryMock.VerifyAll();
    }
    
    [Fact(DisplayName = nameof(ListVideosWithRelations))]
    [Trait("Application", "LIstVideos - Use Cases")]
    public async Task ListVideosWithRelations()
    {
        var (examples, exampleCategories, exampleGenres, exampleCastMembers) = 
            _fixture.CreateExampleVideosListWithRelations();
        var examplesCategoriesIds = exampleCategories.Select(y => y.Id).ToList();
        var examplesGenresIds = exampleGenres.Select(y => y.Id).ToList();
        var examplesCastMembersIds = exampleCastMembers.Select(y => y.Id).ToList();
        var input = new UseCase.ListVideosInput(1, 10, "", "", SearchOrder.Asc);
        var outputRepositorySearch = new SearchOutput<DomainEntity.Video>(
                currentPage: input.Page,
                perPage: input.PerPage,
                items: examples,
                total: examples.Count
                );
        _categoryRepositoryMock.Setup(x => x.GetListByIds(
            It.Is<List<Guid>>(list => list.All(examplesCategoriesIds.Contains)
                && list.Count == examplesCategoriesIds.Count),
            It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategories);
        _genreRepositoryMock.Setup(x => x.GetListByIds(
            It.Is<List<Guid>>(list => list.All(examplesGenresIds.Contains)
                && list.Count == examplesGenresIds.Count),
            It.IsAny<CancellationToken>())).ReturnsAsync(exampleGenres);
        _castMemberRepositoryMock.Setup(x => x.GetListByIds(
            It.Is<List<Guid>>(list => list.All(examplesCastMembersIds.Contains)
                && list.Count == examplesCastMembersIds.Count),
            It.IsAny<CancellationToken>())).ReturnsAsync(exampleCastMembers);
        _videoRepositoryMock.Setup(x => x.SearchAsync(
            It.Is<SearchInput>(s => s.Page == input.Page &&
                s.PerPage == input.PerPage &&
                s.Search == input.Search &&
                s.OrderBy == input.Sort &&
                s.Order == input.Dir
                ),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(outputRepositorySearch);

        PaginatorListOutput<VideoModelOutput> output = await _useCase.Handle(input, CancellationToken.None);

        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(examples.Count);
        output.Items.Should().HaveCount(examples.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleItem = examples.ToList().Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Title.Should().Be(exampleItem!.Title);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.Opened.Should().Be(exampleItem.Opened);
            outputItem.Published.Should().Be(exampleItem.Published);
            outputItem.Duration.Should().Be(exampleItem.Duration);
            outputItem.Rating.Should().Be(exampleItem.Rating.ToStringSignal());
            outputItem.YearLaunched.Should().Be(exampleItem.YearLaunched);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
            outputItem.ThumbFileUrl.Should().Be(exampleItem.Thumb!.Path);
            outputItem.ThumbHalfFileUrl.Should().Be(exampleItem.ThumbHalf!.Path);
            outputItem.BannerFileUrl.Should().Be(exampleItem.Banner!.Path);
            outputItem.VideoFileUrl.Should().Be(exampleItem.Media!.FilePath);
            outputItem.TrailerFileUrl.Should().Be(exampleItem.Trailer!.FilePath);
            outputItem.Categories!.ToList().ForEach(relation =>
            {
                var exampleCategory = exampleCategories.Find(x => x.Id == relation.Id);
                exampleCategory.Should().NotBeNull();
                relation.Name.Should().Be(exampleCategory!.Name);
            });
            outputItem.Genres!.ToList().ForEach(relation =>
            {
                var exampleGenre = exampleGenres.Find(x => x.Id == relation.Id);
                exampleGenre.Should().NotBeNull();
                relation.Name.Should().Be(exampleGenre!.Name);
            });
            outputItem.CastMembers!.ToList().ForEach(relation =>
            {
                var exampleCastMember = exampleCastMembers.Find(x => x.Id == relation.Id);
                exampleCastMember.Should().NotBeNull();
                relation.Name.Should().Be(exampleCastMember!.Name);
            });
        });
        _videoRepositoryMock.VerifyAll();
        _categoryRepositoryMock.VerifyAll();
        _genreRepositoryMock.VerifyAll();
        _castMemberRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(ListVideosWithoutRelationsDoesntCallOthersRepositories))]
    [Trait("Application", "LIstVideos - Use Cases")]
    public async Task ListVideosWithoutRelationsDoesntCallOthersRepositories()
    {
        var examples =
            _fixture.CreateExampleVideosListWithoutRelations();
        var input = new UseCase.ListVideosInput(1, 10, "", "", SearchOrder.Asc);
        var outputRepositorySearch = new SearchOutput<DomainEntity.Video>(
                currentPage: input.Page,
                perPage: input.PerPage,
                items: examples,
                total: examples.Count
                );
        _videoRepositoryMock.Setup(x => x.SearchAsync(
            It.Is<SearchInput>(s => s.Page == input.Page &&
                s.PerPage == input.PerPage &&
                s.Search == input.Search &&
                s.OrderBy == input.Sort &&
                s.Order == input.Dir
                ),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(outputRepositorySearch);

        PaginatorListOutput<VideoModelOutput> output = await _useCase.Handle(input, CancellationToken.None);

        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(examples.Count);
        output.Items.Should().HaveCount(examples.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleItem = examples.ToList().Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Categories.Should().HaveCount(0);
            outputItem.Genres.Should().HaveCount(0);
            outputItem.CastMembers.Should().HaveCount(0);
        });
        _videoRepositoryMock.VerifyAll();
        _categoryRepositoryMock.Verify(x => x.GetListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()), Times.Never);
        _genreRepositoryMock.Verify(x => x.GetListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()), Times.Never);
        _castMemberRepositoryMock.Verify(x => x.GetListByIds(It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(ListReturnsEmptyWhenThereIsNoVideos))]
    [Trait("Application", "LIstVideos - Use Cases")]
    public async Task ListReturnsEmptyWhenThereIsNoVideos()
    {
        var exampleVideoList = new List<DomainEntity.Video>();
        var input = new UseCase.ListVideosInput(1, 10, "", "", SearchOrder.Asc);
        var outputRepositorySearch = new SearchOutput<DomainEntity.Video>(
                currentPage: input.Page,
                perPage: input.PerPage,
                items: exampleVideoList,
                total: exampleVideoList.Count
                );
        _videoRepositoryMock.Setup(x => x.SearchAsync(
            It.Is<SearchInput>(s => s.Page == input.Page &&
                s.PerPage == input.PerPage &&
                s.Search == input.Search &&
                s.OrderBy == input.Sort &&
                s.Order == input.Dir
                ),
            It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(outputRepositorySearch);

        PaginatorListOutput<VideoModelOutput> output = await _useCase.Handle(input, CancellationToken.None);

        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleVideoList.Count);
        output.Items.Should().HaveCount(0);
        _videoRepositoryMock.VerifyAll();
    }

}

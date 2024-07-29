
using Moq;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Repository;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.List;

[Collection(nameof(ListCastMembersFixture))]
public class ListCastMembersTest(ListCastMembersFixture fixture)
{
    [Fact(DisplayName = nameof(List))]
    [Trait("Application", "ListCastMember - Use Cases")]
    public async Task List()
    {
        var castMembersListExample = fixture.GetExampleCastMemberList(3);
        var repositoryMock = new Mock<ICastMemberRepository>();
        SearchOutput<DomainEntity.CastMember> repositorySearchOutput = new(
            1, 10, 3, castMembersListExample.AsReadOnly());
        repositoryMock.Setup(x => x.SearchAsync(It.IsAny<SearchInput>(),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(repositorySearchOutput);
        var input = new ListCastMembersInput(1, 10, "", "", SearchOrder.Asc);
        var useCase = new ListCastMembers(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(repositorySearchOutput.CurrentPage);
        output.PerPage.Should().Be(repositorySearchOutput.PerPage);
        output.Total.Should().Be(repositorySearchOutput.Total);
        output.Items.ToList().ForEach(outputItem =>
        {
            var example = castMembersListExample.Find(x => x.Id == outputItem.Id);
            example.Should().NotBeNull();
            example!.Name.Should().Be(outputItem.Name);
            example.Type.Should().Be(outputItem.Type);
        });
        repositoryMock.Verify(x => x.SearchAsync(
            It.Is<SearchInput>(x => 
                x.Page == input.Page 
                && x.PerPage == input.PerPage
                && x.Search == input.Search
                && x.Order == input.Dir
                && x.OrderBy == input.Sort
            ),
            It.IsAny<CancellationToken>()
            ), Times.Once);
    }

    [Fact(DisplayName = nameof(ReturnsIsEmpty))]
    [Trait("Application", "ListCastMember - Use Cases")]
    public async Task ReturnsIsEmpty()
    {
        var castMembersListExample = new List<DomainEntity.CastMember>();
        var repositoryMock = new Mock<ICastMemberRepository>();
        SearchOutput<DomainEntity.CastMember> repositorySearchOutput = new(
            1, 10, 3, castMembersListExample.AsReadOnly());
        repositoryMock.Setup(x => x.SearchAsync(It.IsAny<SearchInput>(),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(repositorySearchOutput);
        var input = new ListCastMembersInput(1, 10, "", "", SearchOrder.Asc);
        var useCase = new ListCastMembers(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(repositorySearchOutput.CurrentPage);
        output.PerPage.Should().Be(repositorySearchOutput.PerPage);
        output.Total.Should().Be(repositorySearchOutput.Total);
        output.Items.Should().HaveCount(castMembersListExample.Count);
        repositoryMock.Verify(x => x.SearchAsync(
            It.Is<SearchInput>(x =>
                x.Page == input.Page
                && x.PerPage == input.PerPage
                && x.Search == input.Search
                && x.Order == input.Dir
                && x.OrderBy == input.Sort
            ),
            It.IsAny<CancellationToken>()
            ), Times.Once);
    }
}

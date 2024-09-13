using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;
using FC.Codeflix.Catalog.EndToEndTests.Extensions.DateTimes;
using FC.Codeflix.Catalog.EndToEndTests.Models;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

using FluentAssertions;

using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.ListCastMember;

[Collection(nameof(CastMemberApiBaseFixture))]
public class ListCastMemberTest(CastMemberApiBaseFixture fixture) : IDisposable
{
    [Fact(DisplayName = nameof(ListCastMembersAndTotalByDefault))]
    [Trait("EndToEnd/API", "CastMember/GetList - Endpoints")]
    public async Task ListCastMembersAndTotalByDefault()
    {
        var exampleCastMemberList = fixture.GetExampleCastMemberList(5);
        await fixture.Persistence.InsertList(exampleCastMemberList);

        var (response, output) = await fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>(
            "/castmembers");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output!.Data!.Should().NotBeNull();
        output.Meta!.Total.Should().Be(5);
        output.Meta.CurrentPage.Should().Be(1);
        output.Data.Should().HaveCount(5);
        foreach (var outputItem in output.Data!)
        {
            var expectedItem = exampleCastMemberList.First(x => x.Id == outputItem.Id);
            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Type.Should().Be(expectedItem.Type);
        }
    }

    [Fact(DisplayName = nameof(EmptyDbWhenListEmptyInReturn))]
    [Trait("EndToEnd/API", "CastMember/GetList - Endpoints")]
    public async Task EmptyDbWhenListEmptyInReturn()
    {
        
        var (response, output) = await fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>(
            "/castmembers");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output!.Data!.Should().NotBeNull();
        output.Meta!.Total.Should().Be(0);
        output.Meta.CurrentPage.Should().Be(1);
        output.Data.Should().HaveCount(0);
        
    }

    [Theory(DisplayName = nameof(Paginated))]
    [Trait("EndToEnd/API", "CastMember/GetList - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task Paginated(int quantityToGenerate, int page, int perPage, int expectedQuantityItems)
    {

        var examples = fixture.GetExampleCastMemberList(quantityToGenerate);
        var arrangeDbContext = fixture.CreateDbContext();
        await fixture.Persistence.InsertList(examples);
        var input = new ListCastMembersInput(page, perPage, "", "", SearchOrder.Asc);

        var (response, output) = await fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>(
            "/castmembers", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output!.Data!.Should().NotBeNull();
        output.Meta!.Total.Should().Be(quantityToGenerate);
        output.Meta!.PerPage.Should().Be(perPage);
        output.Meta.CurrentPage.Should().Be(page);
        output.Data.Should().HaveCount(expectedQuantityItems);

        foreach (var outputItem in output.Data!)
        {
            var expectedItem = examples.First(x => x.Id == outputItem.Id);
            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Type.Should().Be(expectedItem.Type);
        }
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("EndToEnd/API", "CastMember/GetList - Endpoints")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(string search, int page, int perPage, int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems)
    {

        var namesToGenerate = new List<string>()
        {
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Space",
            "Sci-fi Robots",
            "Sci-fi Future",
        };
        var examples = fixture.GetExampleCastMemberListByNames(namesToGenerate);
        var arrangeDbContext = fixture.CreateDbContext();
        await fixture.Persistence.InsertList(examples);
        var input = new ListCastMembersInput(page, perPage, search, "", SearchOrder.Asc);

        var (response, output) = await fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>(
            "/castmembers", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output!.Data!.Should().NotBeNull();
        output.Meta!.Total.Should().Be(expectedQuantityTotalItems);
        output.Meta!.PerPage.Should().Be(perPage);
        output.Meta.CurrentPage.Should().Be(page);
        output.Data.Should().HaveCount(expectedQuantityItemsReturned);

        foreach (var outputItem in output.Data!)
        {
            var expectedItem = examples.First(x => x.Id == outputItem.Id);
            expectedItem.Should().NotBeNull();
            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.Name.Should().Be(expectedItem.Name);
            outputItem.Type.Should().Be(expectedItem.Type);
        }
    }

    [Theory(DisplayName = nameof(Ordered))]
    [Trait("EndToEnd/API", "CastMember/GetList - Endpoints")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("", "desc")]
    public async Task Ordered(string orderBy, string order)
    {

        var examples = fixture.GetExampleCastMemberList(5);
        examples.ForEach(e => e.CreatedAt.TrimMilliseconds());
        var arrangeDbContext = fixture.CreateDbContext();
        await fixture.Persistence.InsertList(examples);
        var searchOrder = order == "asc"? SearchOrder.Asc: SearchOrder.Desc;
        var input = new ListCastMembersInput(1, 10, "", orderBy, searchOrder);

        var (response, output) = await fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>(
            "/castmembers", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output!.Data!.Should().NotBeNull();
        output.Meta!.Total.Should().Be(5);
        output.Meta!.PerPage.Should().Be(10);
        output.Meta.CurrentPage.Should().Be(1);
        output.Data.Should().HaveCount(5);

        var orderedList = fixture.CloneCastMemberListOrdered(examples, orderBy, searchOrder);
        for (int i = 0; i < orderedList.Count; i++)
        {
            output.Data![i].Name.Should().Be(orderedList[i].Name);
            output.Data[i].Type.Should().Be(orderedList[i].Type);
        }
        
    }

    public void Dispose()
    {
        fixture.CleanPersistence();
    }
}

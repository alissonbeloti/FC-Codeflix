using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace FC.Codeflix.Catalog.IntegrationTest.Infra.Data.EF.Repositories.CastMemberRepository;

[Collection(nameof(CastMemberRepositoryTestFixture))]
public class CastMemberRepositoryTest(CastMemberRepositoryTestFixture fixture)
{
    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "Repositories - CastMemberRepository")]
    public async Task Insert()
    {
        var castMemberExample = fixture.GetExampleCastMember();
        var context = fixture.CreateDbContext();
        var repository =  new Repository.CastMemberRepository(context);

        await repository.Insert(castMemberExample, CancellationToken.None);
        context.SaveChanges();

        var assertionContext = fixture.CreateDbContext(true);
        var castMemberFromDb = assertionContext.CastMembers.AsNoTracking().
            FirstOrDefault(x => x.Id == castMemberExample.Id);
        castMemberFromDb.Should().NotBeNull();
        castMemberFromDb!.Name.Should().Be(castMemberExample.Name);
        castMemberFromDb.Type.Should().Be(castMemberExample.Type);
    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "Repositories - CastMemberRepository")]
    public async Task Get()
    {
        var castMemberExampleList = fixture.GetExampleCastMemberList(5);
        var castMemberExample = castMemberExampleList[3];
        var arrangeContext = fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(castMemberExampleList);
        await arrangeContext.SaveChangesAsync();
        var repository = new Repository.CastMemberRepository(fixture.CreateDbContext(true));

        var itemFromRepository = await repository.Get(castMemberExample.Id, CancellationToken.None);

        itemFromRepository.Should().NotBeNull();
        itemFromRepository!.Name.Should().Be(castMemberExample.Name);
        itemFromRepository.Type.Should().Be(castMemberExample.Type);
    }

    [Fact(DisplayName = nameof(GetThrowWhenNotFound))]
    [Trait("Integration/Infra.Data", "Repositories - CastMemberRepository")]
    public async Task GetThrowWhenNotFound()
    {
        var randomGuid = Guid.NewGuid();
        var repository = new Repository.CastMemberRepository(fixture.CreateDbContext(false));

        var action = async () => await repository.Get(randomGuid, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"CastMember '{randomGuid}' not found.");
    }

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "Repositories - CastMemberRepository")]
    public async Task Delete()
    {
        var castMemberExampleList = fixture.GetExampleCastMemberList(5);
        var castMemberExample = castMemberExampleList[3];
        var arrangeContext = fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(castMemberExampleList);
        await arrangeContext.SaveChangesAsync();
        var actDbContext = fixture.CreateDbContext(true);
        var repository = new Repository.CastMemberRepository(actDbContext);

        await repository.Delete(castMemberExample, CancellationToken.None);
        await actDbContext.SaveChangesAsync();

        var assertionContext = fixture.CreateDbContext(true);
        var itemsInDatabase = assertionContext.CastMembers.AsNoTracking().ToList();
        itemsInDatabase.Should().HaveCount(castMemberExampleList.Count -1);
        itemsInDatabase.Should().NotContain(castMemberExample);
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "Repositories - CastMemberRepository")]
    public async Task Update()
    {
        var castMemberExampleList = fixture.GetExampleCastMemberList(5);
        var castMemberExample = castMemberExampleList[3];
        var newName = fixture.GetValidName();
        var newType = fixture.GetRandomCastMemberType();
        var arrangeContext = fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(castMemberExampleList);
        await arrangeContext.SaveChangesAsync();
        var actDbContext = fixture.CreateDbContext(true);
        var repository = new Repository.CastMemberRepository(actDbContext);

        castMemberExample.Update(newName, newType);
        await repository.Update(castMemberExample, CancellationToken.None);
        await actDbContext.SaveChangesAsync();

        var assertionContext = fixture.CreateDbContext(true);
        var castMemberInDatabase = await assertionContext.CastMembers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Equals(castMemberExample.Id));
        castMemberInDatabase.Should().NotBeNull();
        castMemberInDatabase!.Name.Should().Be(newName);
        castMemberInDatabase.Type.Should().Be(newType);
    }

    [Fact(DisplayName = nameof(Search))]
    [Trait("Integration/Infra.Data", "Repositories - CastMemberRepository")]
    public async Task Search()
    {
        var exampleList = fixture.GetExampleCastMemberList(10);
        var arrangeDbContext = fixture.CreateDbContext(false);
        await arrangeDbContext.AddRangeAsync(exampleList);
        await arrangeDbContext.SaveChangesAsync();
        var actDbContext = fixture.CreateDbContext(true);
        var castMemberRepository = new Repository.CastMemberRepository(actDbContext);

        var searchResult = await castMemberRepository.SearchAsync(new SearchInput(
                1,
                20,
                "",
                "",
                SearchOrder.Asc
            ), CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.Items.Should().HaveCount(10);
        searchResult.Total.Should().Be(exampleList.Count);
        searchResult.CurrentPage.Should().Be(1);
        searchResult.PerPage.Should().Be(20);
        searchResult.Items.Should().BeEquivalentTo(exampleList);
        searchResult.Items.ToList().ForEach(resultItem =>
        {
            var exampleItem = exampleList.Find(x  => x.Id == resultItem.Id);
            exampleItem.Should().NotBeNull();
            exampleItem!.Name.Should().Be(resultItem.Name);
            exampleItem.Type.Should().Be(resultItem.Type);
        });
    }

    [Theory(DisplayName = nameof(SearchWithPagination))]
    [Trait("Integration/Infra.Data", "Repositories - CastMemberRepository")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchWithPagination(int quantityToGenerate, int page, int perPage, int expectedQuantityItems)
    {
        var exampleList = fixture.GetExampleCastMemberList(quantityToGenerate);
        var arrangeDbContext = fixture.CreateDbContext(false);
        await arrangeDbContext.AddRangeAsync(exampleList);
        await arrangeDbContext.SaveChangesAsync();

        var actDbContext = fixture.CreateDbContext(true);
        var castMemberRepository = new Repository.CastMemberRepository(actDbContext);

        var searchResult = await castMemberRepository.SearchAsync(new SearchInput(
                page,
                perPage,
                "",
                "",
                SearchOrder.Asc
            ), CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.Items.Should().HaveCount(expectedQuantityItems);
        searchResult.Total.Should().Be(quantityToGenerate);
        searchResult.CurrentPage.Should().Be(page);
        searchResult.PerPage.Should().Be(perPage);
        searchResult.Items.ToList().ForEach(resultItem =>
        {
            var exampleItem = exampleList.Find(x  => x.Id == resultItem.Id);
            exampleItem.Should().NotBeNull();
            exampleItem!.Name.Should().Be(resultItem.Name);
            exampleItem.Type.Should().Be(resultItem.Type);
        });
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Infra.Data", "Repositories - CastMemberRepository")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(
        string search, int page, int perPage, int expectedQuantityItemsReturned,
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
        var exampleList = fixture.GetExampleCastMemberListByNames(namesToGenerate);
        var arrangeDbContext = fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(exampleList);
        await arrangeDbContext.SaveChangesAsync();

        var actDbContext = fixture.CreateDbContext(true);
        var castMemberRepository = new Repository.CastMemberRepository(actDbContext);

        var searchResult = await castMemberRepository.SearchAsync(new SearchInput(
                page,
                perPage,
                search,
                "",
                SearchOrder.Asc
            ), CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.Items.Should().HaveCount(expectedQuantityItemsReturned);
        searchResult.Total.Should().Be(expectedQuantityTotalItems);
        searchResult.CurrentPage.Should().Be(page);
        searchResult.PerPage.Should().Be(perPage);
        searchResult.Items.ToList().ForEach(resultItem =>
        {
            var exampleItem = exampleList.Find(x => x.Id == resultItem.Id);
            exampleItem.Should().NotBeNull();
            exampleItem!.Name.Should().Be(resultItem.Name);
            exampleItem.Type.Should().Be(resultItem.Type);
        });
    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Infra.Data", "Repositories - CastMemberRepository")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "desc")]
    public async Task SearchOrdered(string orderBy, string order)
    {
        var exampleList = fixture.GetExampleCastMemberList(5);
        var arrangeDbContext = fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(exampleList);
        await arrangeDbContext.SaveChangesAsync();
        var actDbContext = fixture.CreateDbContext(true);
        var castMemberRepository = new Repository.CastMemberRepository(actDbContext);
        var searchOrder = order == "asc"? SearchOrder.Asc: SearchOrder.Desc;

        var searchResult = await castMemberRepository.SearchAsync(new SearchInput(
                1,
                10,
                "",
                orderBy,
                searchOrder
            ), CancellationToken.None);

        var expectedOrderedList = fixture.CloneCastMemberListOrdered(
            exampleList,
            orderBy,
            searchOrder);

        searchResult.Should().NotBeNull();
        searchResult.Items.Should().HaveCount(5);
        searchResult.Total.Should().Be(5);
        searchResult.CurrentPage.Should().Be(1);
        searchResult.PerPage.Should().Be(10);
        for (int i = 0; i < searchResult.Items.Count; i++)
        {
            searchResult.Items[i].Name.Should().Be(expectedOrderedList[i].Name);
            searchResult.Items[i].Type.Should().Be(expectedOrderedList[i].Type);
        }
        
    }

    [Fact(DisplayName = nameof(SearchReturnsEmpty))]
    [Trait("Integration/Infra.Data", "Repositories - CastMemberRepository")]
    public async Task SearchReturnsEmpty()
    {
        var actDbContext = fixture.CreateDbContext();
        var castMemberRepository = new Repository.CastMemberRepository(actDbContext);

        var searchResult = await castMemberRepository.SearchAsync(new SearchInput(
                1,
                20,
                "",
                "",
                SearchOrder.Asc
            ), CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.Items.Should().HaveCount(0);
        searchResult.Total.Should().Be(0);
        searchResult.CurrentPage.Should().Be(1);
        searchResult.PerPage.Should().Be(20);
    }

    [Fact(DisplayName = nameof(GetIdsListByIdsWhenOnlyThreeIdsMatch))]
    [Trait("Integration/Infra.Data", "Repositories - CastMemberRepository")]
    public async Task GetIdsListByIdsWhenOnlyThreeIdsMatch()
    {
        var arrangeDbContext = fixture.CreateDbContext();
        var exampleCastMemberList = fixture.GetExampleCastMemberList(10);
        await arrangeDbContext.AddRangeAsync(exampleCastMemberList);
        await arrangeDbContext.SaveChangesAsync(CancellationToken.None);
        var actDbContext = fixture.CreateDbContext(true);
        var repository = new Repository.CastMemberRepository(actDbContext);
        var idsToGet = new List<Guid>()
        {
            exampleCastMemberList[3].Id,
            exampleCastMemberList[4].Id,
            exampleCastMemberList[5].Id,
            Guid.NewGuid(),
            Guid.NewGuid(),
        };
        var idsExpectedToReturn = new List<Guid>()
        {
            exampleCastMemberList[3].Id,
            exampleCastMemberList[4].Id,
            exampleCastMemberList[5].Id,
        };

        var result = await repository.GetIdsListByIds(idsToGet, CancellationToken.None);

        result.ToList().Should().HaveCount(3);
        result.ToList().Should().NotBeEquivalentTo(idsToGet);
        result.ToList().Should().BeEquivalentTo(idsExpectedToReturn);
    }

    [Fact(DisplayName = nameof(GetListByIds))]
    [Trait("Integration/Infra.Data", "Repositories - CastMemberRepository")]
    public async Task GetListByIds()
    {
        var arrangeDbContext = fixture.CreateDbContext();
        var exampleCastMemberList = fixture.GetExampleCastMemberList(10);
        await arrangeDbContext.AddRangeAsync(exampleCastMemberList);
        await arrangeDbContext.SaveChangesAsync(CancellationToken.None);
        var actDbContext = fixture.CreateDbContext(true);
        var repository = new Repository.CastMemberRepository(actDbContext);
        var idsToGet = new List<Guid>()
        {
            exampleCastMemberList[3].Id,
            exampleCastMemberList[4].Id,
            exampleCastMemberList[5].Id,
        };

        var result = await repository.GetListByIds(idsToGet, CancellationToken.None);

        result.Should().NotBeNull();
        result.ToList().Should().HaveCount(idsToGet.Count);
        idsToGet.ForEach(id =>
        {
            var example = exampleCastMemberList.FirstOrDefault(x => x.Id == id);
            var resultItem = result.FirstOrDefault(x => x.Id == id);
            resultItem.Should().NotBeNull();
            example.Should().NotBeNull();
            resultItem!.Name.Should().Be(example!.Name);
            resultItem.Id.Should().Be(example.Id);
            resultItem.Type.Should().Be(example.Type);
            resultItem.CreatedAt.Should().Be(example.CreatedAt);
        });

    }

    [Fact(DisplayName = nameof(GetListByIdsWhenThreeMatch))]
    [Trait("Integration/Infra.Data", "Repositories - CastMemberRepository")]
    public async Task GetListByIdsWhenThreeMatch()
    {
        var arrangeDbContext = fixture.CreateDbContext();
        var exampleCasMemberList = fixture.GetExampleCastMemberList(10);
        await arrangeDbContext.AddRangeAsync(exampleCasMemberList);
        await arrangeDbContext.SaveChangesAsync(CancellationToken.None);
        var actDbContext = fixture.CreateDbContext(true);
        var repository = new Repository.CastMemberRepository(actDbContext);
        var idsToGet = new List<Guid>()
        {
            exampleCasMemberList[3].Id,
            exampleCasMemberList[4].Id,
            exampleCasMemberList[5].Id,
            Guid.NewGuid(),
            Guid.NewGuid(),
        };
        var expectedIdsToReturn = new List<Guid>()
        {
            exampleCasMemberList[3].Id,
            exampleCasMemberList[4].Id,
            exampleCasMemberList[5].Id,
        };

        var result = await repository.GetListByIds(idsToGet, CancellationToken.None);

        result.Should().NotBeNull();
        result.ToList().Should().HaveCount(expectedIdsToReturn.Count);
        expectedIdsToReturn.ForEach(id =>
        {
            var example = exampleCasMemberList.FirstOrDefault(x => x.Id == id);
            var resultItem = result.FirstOrDefault(x => x.Id == id);
            resultItem.Should().NotBeNull();
            example.Should().NotBeNull();
            resultItem!.Name.Should().Be(example!.Name);
            resultItem.Id.Should().Be(example.Id);
            resultItem.Type.Should().Be(example.Type);
            resultItem.CreatedAt.Should().Be(example.CreatedAt);

        });

    }
}




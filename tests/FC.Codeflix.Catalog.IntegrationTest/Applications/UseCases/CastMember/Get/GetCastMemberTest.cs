using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;
using FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.CastMember.Common;

using FluentAssertions;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.CastMember.Get;
[Collection(nameof(CastMemberUseCaseBaseFixture))]
public class GetCastMemberTest(CastMemberUseCaseBaseFixture fixture)
{
    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Application", "Use Cases - GetCastMember")]
    public async Task Get()
    {
        var examples = fixture.GetExampleCastMemberList(10);
        var example = examples[5];
        var arrangeDbContext = fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(examples);
        await arrangeDbContext.SaveChangesAsync();
        var useCase = new GetCastMember(new CastMemberRepository(fixture.CreateDbContext(true)));
        var input = new GetCastMemberInput(example.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(example.Id);
        output.Name.Should().Be(example.Name);
        output.Type.Should().Be(example.Type);
        output.CreatedAt.Should().Be(example.CreatedAt);
    }

    [Fact(DisplayName = nameof(ThrowNotFound))]
    [Trait("Integration/Application", "Use Cases - GetCastMember")]
    public async Task ThrowNotFound()
    {
        var newGuid = Guid.NewGuid();
        var useCase = new GetCastMember(new CastMemberRepository(fixture.CreateDbContext()));
        var input = new GetCastMemberInput(newGuid);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"CastMember '{newGuid}' not found.");
    }
}

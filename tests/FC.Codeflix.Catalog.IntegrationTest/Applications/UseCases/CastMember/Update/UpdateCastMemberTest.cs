using FC.Codeflix.Catalog.Application.UseCases.CastMember.DeleteCastMember;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.CastMember.Common;

using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.Exceptions;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.CastMember.Update;
[Collection(nameof(CastMemberUseCaseBaseFixture))]
public class UpdateCastMemberTest(CastMemberUseCaseBaseFixture fixture)
{
    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Application", "Use Cases - UpdateCastMember")]
    public async Task Update()
    {
        var examples = fixture.GetExampleCastMemberList(5);
        var example = examples[3];
        var newName = fixture.GetValidName();
        var newType = fixture.GetRandomCastMemberType();
        var arrangeDbContext = fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(examples);
        await arrangeDbContext.SaveChangesAsync();
        var actDbContext = fixture.CreateDbContext(true);
        var useCase = new UpdateCastMember(new CastMemberRepository(actDbContext),
            new UnitOfWork(actDbContext));
        var input = new UpdateCastMemberInput(example.Id, newName, newType);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(example.Id);
        output.Name.Should().Be(newName);
        output.Type.Should().Be(newType);
        output.CreatedAt.Should().Be(example.CreatedAt);

        var assertDbContext = fixture.CreateDbContext(true);
        var castMemberDb = await assertDbContext.CastMembers.AsNoTracking().FirstOrDefaultAsync(
            x => x.Id == input.Id
            );
        castMemberDb.Should().NotBeNull();
        castMemberDb!.Id.Should().Be(example.Id);
        castMemberDb.Name.Should().Be(newName);
        castMemberDb.Type.Should().Be(newType);
        castMemberDb.CreatedAt.Should().Be(example.CreatedAt);
    }

    [Fact(DisplayName = nameof(ThrowsWhenNotFound))]
    [Trait("Integration/Application", "Use Cases - UpdateCastMember")]
    public async Task ThrowsWhenNotFound()
    {
        
        var example = fixture.GetExampleCastMember();
        var newName = fixture.GetValidName();
        var newType = fixture.GetRandomCastMemberType();
        
        var actDbContext = fixture.CreateDbContext(true);
        var useCase = new UpdateCastMember(new CastMemberRepository(actDbContext),
            new UnitOfWork(actDbContext));
        var input = new UpdateCastMemberInput(example.Id, newName, newType);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
           .WithMessage($"CastMember '{example.Id}' not found.");
    }
}

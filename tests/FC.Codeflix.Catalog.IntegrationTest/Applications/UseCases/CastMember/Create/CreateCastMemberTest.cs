using FC.Codeflix.Catalog.Application;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.CastMember.Create;
[Collection(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTest(CreateCastMemberTestFixture fixture)
{
    [Fact(DisplayName = nameof(CreateCastMember))]
    [Trait("Integration/Application", "Use Cases - CreateCastMember")]
    public async Task CreateCastMember()
    {
        var dbContext = fixture.CreateDbContext();
        var repository = new CastMemberRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(dbContext, eventPublisher, 
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());
        var useCase = new CreateCastMember(repository, unitOfWork);
        var input = new CreateCastMemberInput(fixture.GetValidName(), fixture.GetRandomCastMemberType());

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Type.Should().Be(input.Type);
        output.CreatedAt.Should().NotBe(default);
        var assertDbContext = fixture.CreateDbContext(true);
        var castMembers = assertDbContext.CastMembers.AsNoTracking().ToList();
        castMembers.Should().HaveCount(1);
        var castMemberFromDb = castMembers[0];
        castMemberFromDb.Should().NotBeNull();
        castMemberFromDb.Name.Should().Be(input.Name);
        castMemberFromDb.Type.Should().Be(input.Type);
    }
}

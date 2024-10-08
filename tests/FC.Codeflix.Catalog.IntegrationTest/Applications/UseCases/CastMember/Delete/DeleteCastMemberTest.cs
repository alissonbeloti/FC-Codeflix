﻿using FC.Codeflix.Catalog.Application;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.DeleteCastMember;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.CastMember.Common;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.CastMember.Delete;
[Collection(nameof(CastMemberUseCaseBaseFixture))]
public class DeleteCastMemberTest(CastMemberUseCaseBaseFixture fixture)
{
    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Application", "Use Cases - DeleteCastMember")]
    public async Task Delete()
    {
        var examples = fixture.GetExampleCastMemberList(5);
        var example = examples[3];
        var arrangeDbContext = fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(examples);
        await arrangeDbContext.SaveChangesAsync();
        var actDbContext = fixture.CreateDbContext(true);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(actDbContext, eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());
        var useCase = new DeleteCastMember(new CastMemberRepository(actDbContext),
            unitOfWork);
        var input = new DeleteCastMemberInput(example.Id);

        await useCase.Handle(input, CancellationToken.None);
        
        var assertDbContext = fixture.CreateDbContext(true);
        var castMembersDb = await assertDbContext.CastMembers.AsNoTracking().ToListAsync();
        castMembersDb.Find(x => x.Id == example.Id).Should().BeNull();
    }

    [Fact(DisplayName = nameof(ThrowsNotFound))]
    [Trait("Integration/Application", "Use Cases - DeleteCastMember")]
    public async Task ThrowsNotFound()
    {
        var randomGuid = Guid.NewGuid();
        var actDbContext = fixture.CreateDbContext();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(actDbContext, eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());
        var useCase = new DeleteCastMember(new CastMemberRepository(actDbContext),
            unitOfWork);
        var input = new DeleteCastMemberInput(randomGuid);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"CastMember '{randomGuid}' not found.");
    }
}

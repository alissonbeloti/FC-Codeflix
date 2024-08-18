using FC.Codeflix.Catalog.Application;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using UnitOfWorkInfra = FC.Codeflix.Catalog.Infra.Data.EF;
namespace FC.Codeflix.Catalog.IntegrationTest.Infra.Data.EF.UnityOfWork;

[Collection(nameof(UnitOfWorkTestFixture))]
public class UnitOfWorkTest
{
    private readonly UnitOfWorkTestFixture _fixture;

    public UnitOfWorkTest(UnitOfWorkTestFixture fixture)
        => _fixture = fixture;
    
    [Fact(DisplayName = nameof(Commit))]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async Task Commit()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategoriessList = _fixture.GetExampleCategoryList();
        var categoryWithEvent = exampleCategoriessList.First();
        var @event = new DomainEventFake();
        categoryWithEvent.RaiseEvent(@event);
        var eventHandlerMock = new Mock<IDomainEventHandler<DomainEventFake>>();
        await dbContext.AddRangeAsync(exampleCategoriessList);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        serviceCollection.AddSingleton(eventHandlerMock.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(dbContext, eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());

        await unitOfWork.Commit(CancellationToken.None);

        var assertDbContext = _fixture.CreateDbContext(true);
        var savedCategories = assertDbContext.Categories.AsNoTracking().ToList();
        savedCategories.Should().HaveCount(exampleCategoriessList.Count);
        eventHandlerMock.Verify(x => x.HandleAsync(@event, 
            It.IsAny<CancellationToken>()), Times.Once);
        categoryWithEvent.Events.Should().BeEmpty();

    }

    [Fact(DisplayName = nameof(Rollback))]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async Task Rollback()
    {
        var dbContext = _fixture.CreateDbContext();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(dbContext, eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());

        var task = async () => await unitOfWork.Rollback(CancellationToken.None);

        await task.Should().NotThrowAsync();
    }
}

using FC.Codeflix.Catalog.Application;
using FC.Codeflix.Catalog.Domain.SeedWork;

using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application;
public class DomainEventPublisherTest
{
    [Fact(DisplayName = nameof(PublishAsync))]
    [Trait("Application", "DomainEventPublisher")]
    public async Task PublishAsync()
    {
        var serviceCollection = new ServiceCollection();
        var eventHandlerMock1 = new Mock<IDomainEventHandler<DomainEventToBeHandleFake>>();
        var eventHandlerMock2 = new Mock<IDomainEventHandler<DomainEventToBeHandleFake>>();
        var eventHandlerMock3 = new Mock<IDomainEventHandler<DomainEventToNotBeHandleFake>>();
        serviceCollection.AddSingleton(eventHandlerMock1.Object);
        serviceCollection.AddSingleton(eventHandlerMock2.Object);
        serviceCollection.AddSingleton(eventHandlerMock3.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var domainEventPublisher = new DomainEventPublisher(serviceProvider);
        var @event = new DomainEventToBeHandleFake();

        await domainEventPublisher.PublishAsync(@event, CancellationToken.None);

        eventHandlerMock1.Verify(x => x.HandleAsync(@event, It.IsAny<CancellationToken>()), Times.Once);
        eventHandlerMock2.Verify(x => x.HandleAsync(@event, It.IsAny<CancellationToken>()), Times.Once);
        eventHandlerMock3.Verify(x => x.HandleAsync(
            It.IsAny<DomainEventToNotBeHandleFake>(), 
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(PublishAsync))]
    [Trait("Application", "DomainEventPublisher")]
    public async Task NoActionWhenThereIsNoSubscriber()
    {
        var serviceCollection = new ServiceCollection();
        var eventHandlerMock1 = new Mock<IDomainEventHandler<DomainEventToNotBeHandleFake>>();
        var eventHandlerMock2 = new Mock<IDomainEventHandler<DomainEventToNotBeHandleFake>>();
        serviceCollection.AddSingleton(eventHandlerMock1.Object);
        serviceCollection.AddSingleton(eventHandlerMock2.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var domainEventPublisher = new DomainEventPublisher(serviceProvider);
        var @event = new DomainEventToBeHandleFake();

        await domainEventPublisher.PublishAsync(@event, CancellationToken.None);

        eventHandlerMock1.Verify(x => x.HandleAsync(It.IsAny<DomainEventToNotBeHandleFake>(), 
            It.IsAny<CancellationToken>()), Times.Never);
        eventHandlerMock2.Verify(x => x.HandleAsync(It.IsAny<DomainEventToNotBeHandleFake>(), 
            It.IsAny<CancellationToken>()), Times.Never);

    }
}

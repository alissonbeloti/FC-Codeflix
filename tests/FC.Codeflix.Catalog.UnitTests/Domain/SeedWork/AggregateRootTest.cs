using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.SeedWork;
public class AggregateRootTest
{
    [Fact(DisplayName = nameof(RaiseEvent))]
    [Trait("Domaing", "AggregateRoot - SeedWork")]
    public void RaiseEvent()
    {
        var domainEvent = new DomainEventFake();
        var aggreagate = new AggregateRootFake();

        aggreagate.RaiseEvent(domainEvent);

        aggreagate.Events.Should().HaveCount(1);
    }

    [Fact(DisplayName = nameof(ClearEvents))]
    [Trait("Domaing", "AggregateRoot - SeedWork")]
    public void ClearEvents()
    {
        var domainEvent = new DomainEventFake();
        var aggreagate = new AggregateRootFake();
        aggreagate.RaiseEvent(domainEvent);

        aggreagate.ClearEvents();

        aggreagate.Events.Should().BeEmpty();
    }
}

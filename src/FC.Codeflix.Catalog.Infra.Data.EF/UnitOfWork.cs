using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;

using Microsoft.Extensions.Logging;

namespace FC.Codeflix.Catalog.Infra.Data.EF;
public class UnitOfWork
    : IUnitOfWork
{
    private readonly CodeflixCatalogDbContext _context;
    private readonly IDomainEventPublisher _publisher;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(CodeflixCatalogDbContext context, IDomainEventPublisher publisher,
        ILogger<UnitOfWork> logger)
    {
        _context = context;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task Commit(CancellationToken cancellationToken)
    {
        var aggregateRoots = _context.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(entry => entry.Entity.Events.Any())
            .Select(entry => entry.Entity);
        _logger.LogInformation("Commit: {AggregatesCount} aggregate roots with events."
            , aggregateRoots.Count());
        var events = aggregateRoots.SelectMany(entry => entry.Events);
        _logger.LogInformation("Commit: {eventsCount} events raised.", events.Count());
        foreach (var @event in events)
        {
            await _publisher.PublishAsync((dynamic)@event, cancellationToken);
        }
        foreach (var aggregate in aggregateRoots)
        {
            aggregate.ClearEvents();
        }
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task Rollback(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}

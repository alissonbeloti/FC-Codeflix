using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Events;
using FC.Codeflix.Catalog.Domain.SeedWork;

namespace FC.Codeflix.Catalog.Application.EventHandlers;
public class SendToEncoderEventHandler(IMessageProducer messageProducer)
    : IDomainEventHandler<VideoUploadedEvent>
{
    public async Task HandleAsync(VideoUploadedEvent domainEvent, CancellationToken cancellationToken)
    {
        await messageProducer.SendMessageAsync(domainEvent, cancellationToken);
    }
}

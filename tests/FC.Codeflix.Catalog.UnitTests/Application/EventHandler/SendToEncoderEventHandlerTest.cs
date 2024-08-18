using Moq;
using FC.Codeflix.Catalog.Domain.Events;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.EventHandlers;

namespace FC.Codeflix.Catalog.UnitTests.Application.EventHandler;
public class SendToEncoderEventHandlerTest
{
    [Fact(DisplayName = nameof(HandleAsync))]
    [Trait("Application", "EventHandlers - SendToEncoderEventHandlerTest")]
    public async Task HandleAsync()
    {
        var messageProducerMock = new Mock<IMessageProducer>();
        messageProducerMock.Setup(x => x.SendMessageAsync(It.IsAny<VideoUploadedEvent>()
            , It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var handler = new SendToEncoderEventHandler(messageProducerMock.Object);
        VideoUploadedEvent @event = new(Guid.NewGuid(), "medias/video.mp4");

        await handler.HandleAsync(@event, CancellationToken.None);

        messageProducerMock.Verify(x => x.SendMessageAsync(@event, 
            It.IsAny<CancellationToken>()), Times.Once);
    }
}

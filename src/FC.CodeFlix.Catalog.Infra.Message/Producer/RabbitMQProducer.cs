using FC.Codeflix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Infra.Message.Configuration;
using FC.CodeFlix.Catalog.Infra.Message.JsonPolicies;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;

using System.Text.Json;

namespace FC.CodeFlix.Catalog.Infra.Message.Producer;
public class RabbitMQProducer : IMessageProducer
{
    private readonly IModel _channel;
    private readonly string _exchange;

    public RabbitMQProducer(IModel channel, IOptions<RabbitMQConfiguration> options)
    {
        _channel = channel;
        _exchange = options.Value.Exchange!;
    }

    public Task SendMessageAsync<T>(T message, CancellationToken cancellationToken)
    {
        var routingKey = EventsMapping.GetRoutingKey<T>();
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new JsonSnakeCasePolicy()
        };
        var @event = JsonSerializer.SerializeToUtf8Bytes(message, jsonOptions);
        _channel.BasicPublish(
            _exchange,
            routingKey: routingKey,
            body: @event);
        _channel.WaitForConfirmsOrDie(); //em muitos eventos se retirar pode gerar mais performance. Porém pode-se perder perfomance.
        return Task.CompletedTask;
    }
}

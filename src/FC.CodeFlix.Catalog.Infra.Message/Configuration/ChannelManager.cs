using RabbitMQ.Client;

namespace FC.CodeFlix.Catalog.Infra.Message.Configuration;
public class ChannelManager(IConnection connection)
{
    private IModel? _channel = null;
    private readonly object _lock = new();
    public IModel GetChannel()
    {
        lock (_lock)
        {
            if (_channel == null || _channel.IsClosed)
            {
                _channel = connection.CreateModel();
                _channel.ConfirmSelect();
            }
            return _channel;
        }
    }
}

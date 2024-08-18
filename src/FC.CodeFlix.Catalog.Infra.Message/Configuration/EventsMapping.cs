using FC.Codeflix.Catalog.Domain.Events;

namespace FC.CodeFlix.Catalog.Infra.Message.Configuration;
public class EventsMapping
{
    private static Dictionary<string, string> _routingKeys = new()
    {
        {typeof(VideoUploadedEvent).Name, "video.created" }
    };

    public static string GetRoutingKey<T>() => _routingKeys[typeof(T).Name];
}

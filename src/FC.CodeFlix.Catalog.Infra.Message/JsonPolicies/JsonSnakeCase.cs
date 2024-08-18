using FC.CodeFlix.Catalog.Infra.Message.Extensions;

using System.Text.Json;

namespace FC.CodeFlix.Catalog.Infra.Message.JsonPolicies;

public class JsonSnakeCasePolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
        => name.ToSnakeCase();

}

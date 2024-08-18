using Newtonsoft.Json.Serialization;

namespace FC.CodeFlix.Catalog.Infra.Message.Extensions;

public static class SnakeCaseExtension
{
    private readonly static NamingStrategy _snakeCaseNamingStrategy =
        new SnakeCaseNamingStrategy();

    public static string ToSnakeCase(this string strintToConvert)
    {
        ArgumentNullException.ThrowIfNull(nameof(strintToConvert));
        return _snakeCaseNamingStrategy.GetPropertyName(strintToConvert, false);
    }
}

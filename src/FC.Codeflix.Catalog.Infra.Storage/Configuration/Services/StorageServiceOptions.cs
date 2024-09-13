namespace FC.Codeflix.Catalog.Infra.Storage.Configuration.Services;
public class StorageServiceOptions
{
    public const string ConfigurationSection = "Storage";
    public string BucketName { get; set; }
}

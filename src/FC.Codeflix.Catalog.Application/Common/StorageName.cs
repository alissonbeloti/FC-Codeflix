namespace FC.Codeflix.Catalog.Application.Common;
public static class StorageName
{
    public static string Create(Guid id, string propertyName, string extension)
        => $"{id}/{propertyName.ToLower()}.{extension}";
}

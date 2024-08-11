using FC.Codeflix.Catalog.Application.Common;

using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Common;
public class StoragePathNameTest
{
    [Fact()]
    [Trait("Application", "StoragePathName - Common")]
    public void CreateStorageNameForFile()
    {
        var exampleId = Guid.NewGuid();
        var exampleExtension = "mp4";
        var propertyName = "Video";

        var name = StorageName.Create(exampleId, propertyName, exampleExtension);

        name.Should().Be($"{exampleId}-{propertyName.ToLower()}.{exampleExtension}");

    }
}

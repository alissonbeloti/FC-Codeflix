using FC.Codeflix.Catalog.UnitTests.Common;

namespace FC.Codeflix.Catalog.UnitTests.Infra;
[CollectionDefinition(nameof(StorageServiceTestFixture))]
public class StorageServiceTestFixtureCollection: ICollectionFixture<StorageServiceTestFixture> { }
public class StorageServiceTestFixture: BaseFixture
{
    public string GetBucketName()
        => "fc3-catalog-medias";

    public string GetFileName()
        => Faker.System.CommonFileName();

    public string GetContentFile() => Faker.Lorem.Paragraph();

    public string GetContenteType() => Faker.System.MimeType();
}

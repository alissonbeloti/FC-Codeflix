using Google.Apis.Upload;
using Google.Cloud.Storage.V1;
using GcpData = Google.Apis.Storage.v1.Data;
using Microsoft.Extensions.Options;
using InfraStorage = FC.Codeflix.Catalog.Infra.Storage.Configuration.Services;
using Moq;
using System.Text;
using Google.Apis.Storage.v1;
using FluentAssertions;
using FC.Codeflix.Catalog.Infra.Storage.Configuration.Services;

namespace FC.Codeflix.Catalog.UnitTests.Infra;

[Collection(nameof(StorageServiceTestFixture))]
public class StorageServiceTest(StorageServiceTestFixture fixture)
{
    [Fact(DisplayName = nameof(Upload))]
    [Trait("Infra.Storage", "StorageService")]
    private async Task Upload()
    {
        var storageClientMock = new Mock<StorageClient>();
        var objectMock = new Mock<GcpData.Object>();
        storageClientMock.Setup(x => x.UploadObjectAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<Stream>(), It.IsAny<UploadObjectOptions>(),
            It.IsAny<CancellationToken>(), It.IsAny<IProgress<IUploadProgress>>()))
            .ReturnsAsync(objectMock.Object);
        var storageOptions = new InfraStorage.StorageServiceOptions
        {
            BucketName = fixture.GetBucketName()
        };
        var options = Options.Create(storageOptions);
        var service = new InfraStorage.StorageService(storageClientMock.Object, options);
        var fileName = fixture.GetFileName();
        var contentStream = Encoding.UTF8.GetBytes(fixture.GetContentFile());
        var stream = new MemoryStream(contentStream);
        var contentType = fixture.GetContenteType();

        var filePath = await service.Upload(fileName, stream, contentType, 
            CancellationToken.None);

        filePath.Should().Be(fileName);
        storageClientMock.Verify(x => x.UploadObjectAsync(
            storageOptions.BucketName, fileName, contentType,
            stream, It.IsAny<UploadObjectOptions>(),
            It.IsAny<CancellationToken>(), It.IsAny<IProgress<IUploadProgress>>()), Times.Once);
    }

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Infra.Storage", "StorageService")]
    private void Delete()
    {
        var storageClientMock = new Mock<StorageClient>();
        storageClientMock.Setup(x => x.DeleteObjectAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DeleteObjectOptions>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var storageOptions = new InfraStorage.StorageServiceOptions
        {
            BucketName = fixture.GetBucketName()
        };
        var options = Options.Create(storageOptions);
        var service = new InfraStorage.StorageService(storageClientMock.Object, options);
        var fileName = fixture.GetFileName();
        
        var filePath = service.Delete(fileName, CancellationToken.None);

        storageClientMock.Verify(x => x.DeleteObjectAsync(
            storageOptions.BucketName, fileName, It.IsAny<DeleteObjectOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    //[Fact]
    //public async Task IntegrationTest()
    //{
    //    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
    //        "H:\\estudos\\fullcycle\\FC.Codeflix.Catalog\\src\\FC.Codeflix.Catalog.Api\\gcp_credentials.json");
    //    var client = StorageClient.Create();
    //    var storageOptions = new StorageServiceOptions
    //    {
    //        BucketName = "fc3-medias-catalog-admin-dotnet-alisson",
    //    };
    //    var options = Options.Create(storageOptions);
    //    var service = new InfraStorage.StorageService(client, options);
    //    var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello World"));
    //    await service.Upload("teste/file.txt", stream, "text/plain", CancellationToken.None);
    //}
}

using Bogus;
using FC.Codeflix.Catalog.Domain.Entity;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using System.Text;

namespace FC.Codeflix.Catalog.UnitTests.Common.Fixtures;
public abstract class VideoTestFixtureBase: BaseFixture 
{
    public string GetValidDescription()
        => Faker.Commerce.ProductDescription();

    public int GetValidDuration()
        => (new Random()).Next(80, 300);


    public string GetValidTitle()
        => (Faker.Commerce.Product() + Faker.Lorem.Letter(100)).Substring(0, 100);
    public string GetInvalidTooLongTitle()
        => (Faker.Commerce.Product() + Faker.Lorem.Letter(300));

    public string GetValidImagePath()
        => Faker.Image.PlaceImgUrl();

    public string GetValidMediaPath()
    {
        var exampleMedias = new String[]
        {
            "https://www.googlestorage.com/file-example.mp4",
            "https://www.storage.com/file-example.mp4",
            "https://www.s3.com.br/file-example.mp4",
            "https://www.glg.io/file-example.mp4",
        };
        var random = new Random();
        return exampleMedias[random.Next(exampleMedias.Length)];
    }

    public DomainEntity.Video GetValidVideo()
        => new DomainEntity.Video(
            GetValidTitle(),
            GetValidDescription(),
            GetRandoBoolean(),
            GetRandoBoolean(),
            GetValidYearLaunched(),
            GetValidDuration(),
            GetRandomRationg()
        );

    public Rating GetRandomRationg()
    {
        var values = Enum.GetValues<Rating>();
        var random = new Random();
        return values[random.Next(values.Length)];
    }

    public int GetValidYearLaunched()
        => Faker.Date.BetweenDateOnly(new DateOnly(1960, 1, 1), new DateOnly(2024, 1, 1)).Year;

    internal string GetTooLongTitle()
        => Faker.Lorem.Letter(400);

    internal string GetTooLongDescription()
        => Faker.Lorem.Letter(4001);

    internal Media GetValidMedia() =>
        new Media(GetValidMediaPath());

    internal FileInput GetValidImageFileInput()
    {
        var exampleStream = new MemoryStream(Encoding.ASCII.GetBytes("teste"));
        var fileInput = new FileInput("jpg", exampleStream);
        return fileInput;
    }

    internal FileInput GetValidMediaFileInput()
    {
        var exampleStream = new MemoryStream(Encoding.ASCII.GetBytes("teste"));
        var fileInput = new FileInput("mp4", exampleStream);
        return fileInput;
    }
}

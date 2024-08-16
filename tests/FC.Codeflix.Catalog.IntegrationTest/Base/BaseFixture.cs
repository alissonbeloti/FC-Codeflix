using Bogus;

using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;

using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.IntegrationTest.Base;

public class BaseFixture
{
    protected Faker Faker { get; set; }

    public BaseFixture() => Faker = new Faker("pt_BR");

    public CodeflixCatalogDbContext CreateDbContext(bool preserveData = false)
    {
        var dbContext = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
            .UseInMemoryDatabase($"integration-tests-db")
            .Options
            );
        if (!preserveData)
        {
            dbContext.Database.EnsureDeleted();
        }
        return dbContext;
    }
    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
        {
            categoryName = Faker.Commerce.Categories(1)[0];
        }

        if (categoryName.Length > 255)
        {
            categoryName = categoryName[..255];
        }

        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();

        if (categoryDescription.Length > 10_000)
        {
            categoryDescription = categoryDescription[..10_000];
        }
        return categoryDescription;
    }

    public string GetValidName()
        => Faker.Name.FullName();

    public CastMemberType GetRandomCastMemberType()
        => (CastMemberType)(new Random()).Next(1, 2);

    public bool GetRandoBoolean()
        => (new Random()).NextDouble() < 0.5;

    public Category GetExampleCategory()
        => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandoBoolean());

    public List<Category> GetExampleCategoryList(int length = 10)
        => Enumerable.Range(1, length).Select(_ => GetExampleCategory()).ToList();

    public CastMember GetExampleCastMember(string? name = null)
       => new CastMember(
           name ?? GetValidName(),
           GetRandomCastMemberType());

    internal List<CastMember> GetExampleCastMemberList(int count)
        => Enumerable.Range(1, count).Select(_ => GetExampleCastMember()).ToList();

    internal List<Genre> GetExampleGenreList(int count)
        => Enumerable.Range(1, count).Select(_ => GetValidGenre()).ToList();

    public Video GetExampleVideo(string? title = null) => new(
            title ?? GetValidTitle(),
            GetValidDescription(),
            GetRandoBoolean(),
            GetRandoBoolean(),
            GetValidYearLaunched(),
            GetValidDuration(),
            GetRandomRating()
        );

   

    public Rating GetRandomRating()
    {
        var values = Enum.GetValues<Rating>();
        var random = new Random();
        return values[random.Next(values.Length)];
    }

    public int GetValidYearLaunched()
        => Faker.Date.BetweenDateOnly(new DateOnly(1960, 1, 1), new DateOnly(2024, 1, 1)).Year;
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

    public string GetValidGenreName()
    {
        var name = "";
        while (name.Length < 3)
        {
            name = Faker.Commerce.Categories(1)[0];
        }

        if (name.Length > 255)
        {
            name = name[..255];
        }

        return name;
    }


    public Genre GetValidGenre(List<Guid>? categoriesList = null)
    {
        Genre genre = new(GetValidGenreName(), GetRandoBoolean());
        if (categoriesList != null)
        {
            foreach (var id in categoriesList)
            {
                genre.AddCategory(id);
            }
        }
        return genre;
    }

    public Video GetValidVideoWithAllProperties()
    {
        var video = new Video(
                GetValidTitle(),
                GetValidDescription(),
                GetRandoBoolean(),
                GetRandoBoolean(),
                GetValidYearLaunched(),
                GetValidDuration(),
                GetRandomRating()
            );

        video.UpdateBanner(GetValidImagePath());
        video.UpdateThumb(GetValidImagePath());
        video.UpdateThumbHalf(GetValidImagePath());
        video.UpdateMedia(GetValidMediaPath());
        video.UpdateTrailer(GetValidMediaPath());

        video.UpdateAsEncoded(GetValidMediaPath());

        var random = new Random();
        Enumerable.Range(1, random.Next(2, 5)).ToList()
            .ForEach(_ => video.AddCastMember(Guid.NewGuid()));
        Enumerable.Range(1, random.Next(2, 5)).ToList()
            .ForEach(_ => video.AddCategory(Guid.NewGuid()));
        Enumerable.Range(1, random.Next(2, 5)).ToList()
            .ForEach(_ => video.AddGenre(Guid.NewGuid()));

        return video;
    }
}

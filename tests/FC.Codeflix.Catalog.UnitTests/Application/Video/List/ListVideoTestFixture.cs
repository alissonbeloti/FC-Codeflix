using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;
using FC.Codeflix.Catalog.Domain.Enum;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.List;
[CollectionDefinition(nameof(ListVideoTestFixture))]
public class ListVideoTestFixtureCollection : ICollectionFixture<ListVideoTestFixture> { }
public class ListVideoTestFixture : VideoTestFixtureBase
{
    public List<DomainEntity.Video> CreateExampleVideosListWithoutRelations() =>
        Enumerable.Range(1, Random.Shared.Next(2, 10))
            .Select(_ => GetValidVideo()).ToList();
    public List<DomainEntity.Video> CreateExampleVideosList() =>
        Enumerable.Range(1, Random.Shared.Next(2, 10))
            .Select(_ => GetValidVideoWithAllProperties()).ToList();

    public (IReadOnlyList<DomainEntity.Video> Videos, 
        List<DomainEntity.Category> Categories,
        List<DomainEntity.Genre> Genres,
        List<DomainEntity.CastMember> CastMembers
        ) 
        CreateExampleVideosListWithRelations()
    {
        var itemsToBeCreated = Random.Shared.Next(2, 10);
        List<DomainEntity.Category> categories = new List<DomainEntity.Category>();
        List<DomainEntity.Genre> genres = new List<DomainEntity.Genre>();
        List<DomainEntity.CastMember> castMembers = new List<DomainEntity.CastMember>();
        var videos = Enumerable.Range(1, Random.Shared.Next(2, itemsToBeCreated))
            .Select(_ => GetValidVideoWithAllProperties()).ToList();
        videos.ForEach(video =>
        {
            video.RemoveAllCategories();
            var qtdCategories = Random.Shared.Next(2, 5);
            for (int i = 0; i < qtdCategories; i++)
            {
                var category = GetExampleCategory();
                categories.Add(category);
                video.AddCategory(category.Id);
            }

            video.RemoveAllGenres();
            var qtdGenres = Random.Shared.Next(2, 5);
            for (int i = 0; i < qtdGenres; i++)
            {
                var genre = GetExampleGenre();
                genres.Add(genre);
                video.AddGenre(genre.Id);
            }

            video.RemoveAllCastMembers();
            var qtdCastMembers = Random.Shared.Next(2, 5);
            for (int i = 0; i < qtdCastMembers; i++)
            {
                var castMember = GetExampleCastMember();
                castMembers.Add(castMember);
                video.AddCastMember(castMember.Id);
            }
        });

        return (videos, categories, genres, castMembers);
    }

    string GetValidCategoryName()
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

    string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();

        if (categoryDescription.Length > 10_000)
        {
            categoryDescription = categoryDescription[..10_000];
        }
        return categoryDescription;
    }

    DomainEntity.Category GetExampleCategory()
        => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandoBoolean());

    string GetValidGenreName()
        => Faker.Commerce.Categories(1)[0];

    DomainEntity.Genre GetExampleGenre() => 
        new DomainEntity.Genre(GetValidGenreName(),
        GetRandoBoolean());

    DomainEntity.CastMember GetExampleCastMember()
        => new DomainEntity.CastMember(
            GetValidName(),
            GetRandomCastMemberType());

    string GetValidName()
        => Faker.Name.FullName();

    CastMemberType GetRandomCastMemberType()
        => (CastMemberType)(new Random()).Next(1, 2);

}

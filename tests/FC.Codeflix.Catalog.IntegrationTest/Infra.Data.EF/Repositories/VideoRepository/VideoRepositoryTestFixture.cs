using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.IntegrationTest.Base;

namespace FC.Codeflix.Catalog.IntegrationTest.Infra.Data.EF.Repositories.VideoRepository;
[CollectionDefinition(nameof(VideoRepositoryTestFixture))]
public class VideoRepositoryTestFixtureCollection : ICollectionFixture<VideoRepositoryTestFixture> { }
public class VideoRepositoryTestFixture : BaseFixture
{
    public List<Video> GetExampleVideoList(int count = 10)
   => Enumerable.Range(1, count).Select(_ => GetExampleVideo()).ToList();

    public List<Video> GetExampleVideoListByNames(List<string> names)
   => names.Select(name => GetExampleVideo(name)).ToList();

    public List<Video> GetExampleVideoAllPropertiesList(int count = 10)
   => Enumerable.Range(1, count).Select(_ => GetValidVideoWithAllProperties()).ToList();

    public List<Video> CloneListOrdered(List<Video> videoList, SearchInput input)
    {
        var listClone = new List<Video>(videoList);
        var orderedEnumerable = (input.OrderBy.ToLower(), input.Order) switch
        {
            ("title", SearchOrder.Asc) => listClone.OrderBy(x => x.Title).ThenBy(x => x.Id),
            ("title", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Title)
                .ThenByDescending(x => x.Id),
            ("id", SearchOrder.Asc) => listClone.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
            _ => listClone.OrderBy(x => x.Title)
                .ThenBy(x => x.Id),
        };
        return orderedEnumerable.ToList();
    }
}

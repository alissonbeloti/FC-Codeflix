using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Category.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Category.ListCategory;

[CollectionDefinition(nameof(ListCategoryTestFixture))]
public class ListCategoryTestFixtureCollection : ICollectionFixture<ListCategoryTestFixture>
{ }

public class ListCategoryTestFixture: CategoryUseCaseFixture
{
    public List<DomainEntity.Category> GetExampleCategoriesListWithName(List<string> names)
        => names.Select(name =>
        {
            var category = GetExampleCategory();
            category.Update(name);
            return category;
        }).ToList();

    public List<DomainEntity.Category> CloneCategoriesListOrdered(
        List<DomainEntity.Category> categories, 
        string orderBy, 
        SearchOrder order)
    {
        var listClone = new List<DomainEntity.Category>(categories);
        var orderedEnumerable = (orderBy.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => listClone.OrderBy(x => x.Name).ThenBy(x => x.Id),
            ("name", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Name)
                .ThenByDescending(x => x.Id),
            ("id", SearchOrder.Asc) => listClone.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
            _ => listClone.OrderBy(x => x.Name)
                .ThenBy(x => x.Id),
        };
        return orderedEnumerable.ToList();
    }
}

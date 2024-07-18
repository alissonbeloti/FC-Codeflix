using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.IntegrationTest.Base;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;


namespace FC.Codeflix.Catalog.IntegrationTest.Infra.Data.EF.Repositories.CategoryRepository;
[CollectionDefinition(nameof(CategoryRepositoryTestFixture))]
public class CategoryRepositoryTestFixtureCollection : ICollectionFixture<CategoryRepositoryTestFixture>
{ }

public class CategoryRepositoryTestFixture : BaseFixture
{
    public List<Category> GetExampleCategoriesListWithName(List<string> names)
        => names.Select(name =>
        {
            var category = GetExampleCategory();
            category.Update(name);
            return category;
        }).ToList();

    public List<Category> CloneCategoriesListOrdered(List<Category> categories, string orderBy, SearchOrder order)
    {
        var listClone = new List<Category>(categories);
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

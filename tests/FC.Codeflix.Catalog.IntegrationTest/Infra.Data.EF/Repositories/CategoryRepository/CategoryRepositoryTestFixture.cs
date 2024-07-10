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
            ("name", SearchOrder.Asc) => listClone.OrderBy(x => x.Name),   
            ("name", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Name),
            ("id", SearchOrder.Asc) => listClone.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
            ("cretatedat", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
            ("cretatedat", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
            _ => listClone.OrderBy(x => x.Name),
        };
        return orderedEnumerable.ToList();
    }

}

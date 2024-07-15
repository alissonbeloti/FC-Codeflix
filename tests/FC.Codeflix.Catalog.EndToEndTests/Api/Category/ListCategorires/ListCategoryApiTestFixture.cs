﻿using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.ListCategorires;
[CollectionDefinition(nameof(ListCategoryApiTestFixture))]
public class ListCategoryApiTestFixtureCollection : ICollectionFixture<ListCategoryApiTestFixture> {}
public class ListCategoryApiTestFixture : CategoryBaseFixture
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
            ("name", SearchOrder.Asc) => listClone.OrderBy(x => x.Name),
            ("name", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Name),
            ("id", SearchOrder.Asc) => listClone.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
            ("cretatedat", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
            ("cretatedat", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
            _ => listClone.OrderBy(x => x.Name),
        };
        orderedEnumerable = orderedEnumerable.ThenBy(x => x.CreatedAt); 
        return orderedEnumerable.ToList();
    }
}

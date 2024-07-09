using FC.Codeflix.Catalog.Application.Common;
using MediatR;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;


namespace FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
public class ListCategoriesInput : PaginatorListInput, IRequest<ListCategoriesOutput>
{
    public ListCategoriesInput(int page, int perPage, string search, string sort, SearchOrder dir) 
        : base(page, perPage, search, sort, dir)
    {
    }
}

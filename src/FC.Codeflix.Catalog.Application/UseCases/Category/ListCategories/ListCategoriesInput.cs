using MediatR;
using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;


namespace FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
public class ListCategoriesInput : PaginatorListInput, IRequest<ListCategoriesOutput>
{
    public ListCategoriesInput(int page = 1, int perPage = 15, 
        string search = "", string sort = "", SearchOrder dir = SearchOrder.Asc) 
        : base(page, perPage, search, sort, dir)
    {
    }
    public ListCategoriesInput()
        : base(1, 15, "", "", SearchOrder.Asc)
    {
    }
}

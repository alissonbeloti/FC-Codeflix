using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.ListVideo;
public class ListVideosInput : PaginatorListInput, IRequest<ListVideoOutput>
{
    public ListVideosInput(int page = 1, 
        int perPage = 15, 
        string search = "", 
        string sort = "", 
        SearchOrder dir = SearchOrder.Asc) 
        : base(page, perPage, search, sort, dir)
    {
    }
}

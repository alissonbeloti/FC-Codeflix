using FC.Codeflix.Catalog.Application.Common;

namespace FC.Codeflix.Catalog.Api.ApiModels.Response;

public class ApiResponseList<TItemData> : ApiResponse<IReadOnlyList<TItemData>>
{
    public ApiResponseListMeta Meta { get; private set; }
    public ApiResponseList( 
        int currentPage, int perPage, int total,
        IReadOnlyList<TItemData> data) : base(data)
    {
        Meta = new ApiResponseListMeta(currentPage, perPage, total);
    }

    public ApiResponseList(PaginatorListOutput<TItemData> paginator
        ) : base(paginator.Items)
    {
        Meta = new ApiResponseListMeta(paginator.Page, paginator.PerPage, paginator.Total);
    }
}

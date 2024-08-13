using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.ListVideo;
public class ListVideoOutput : PaginatorListOutput<VideoModelOutput>
{

    public ListVideoOutput(int page, 
        int perPage, 
        int total, 
        IReadOnlyList<VideoModelOutput> items) 
        : base(page, perPage, total, items)
    { }
}

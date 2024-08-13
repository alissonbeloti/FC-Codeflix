using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.ListVideo;
public interface IListVideos : IRequestHandler<ListVideosInput, ListVideoOutput>
{
}

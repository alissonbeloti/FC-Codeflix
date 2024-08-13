
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.GetVideo;
public class GetVideo(IVideoRepository repository) : IGetVideo
{
    public async Task<VideoModelOutput> Handle(GetVideoInput request, CancellationToken cancellationToken)
    {
        var video = await repository.Get(request.VideoId, cancellationToken);
        return VideoModelOutput.FromVideo(video);
    }
}

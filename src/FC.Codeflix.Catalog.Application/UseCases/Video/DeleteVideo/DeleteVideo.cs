using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.DeleteVideo;
public class DeleteVideo(IVideoRepository repository, 
    IUnitOfWork unitOfWork,
    IStorageService storageService) 
    : IDeleteVideo
{
    public async Task Handle(DeleteVideoInput request, CancellationToken cancellationToken)
    {
        var video = await repository.Get(request.VideoId, cancellationToken);

        await repository.Delete(video, cancellationToken);
        await unitOfWork.Commit(cancellationToken);

        if (video.Trailer is not null )
        {
            await storageService.Delete(video.Trailer.FilePath, cancellationToken);
        }

        if (video.Media is not null)
        {
            await storageService.Delete(video.Media.FilePath, cancellationToken);
        }
    }
}

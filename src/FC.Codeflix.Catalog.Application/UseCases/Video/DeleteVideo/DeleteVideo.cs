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
        var trailerFilePath = video.Trailer?.FilePath;
        var mediaFilePath = video.Media?.FilePath;
        var bannerFilePath = video.Banner?.Path;
        var thumbFilePath = video.Thumb?.Path;
        var thumbHalfFilePath = video.ThumbHalf?.Path;

        await repository.Delete(video, cancellationToken);
        await unitOfWork.Commit(cancellationToken);

        await ClearVideoMedias(storageService, trailerFilePath, mediaFilePath, cancellationToken);
        await ClearVideoImages(storageService, bannerFilePath, thumbFilePath, thumbHalfFilePath, cancellationToken);
    }

    private static async Task ClearVideoMedias(IStorageService storageService, string? trailerFilePath, string? mediaFilePath, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(trailerFilePath))
        {
            await storageService.Delete(trailerFilePath!, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(mediaFilePath))
        {
            await storageService.Delete(mediaFilePath!, cancellationToken);
        }
    }

    private static async Task ClearVideoImages(IStorageService storageService, 
        string? bannerFilePath, 
        string? thumbFilePath,
        string? thumbHalfFilePath,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(bannerFilePath))
        {
            await storageService.Delete(bannerFilePath!, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(thumbFilePath))
        {
            await storageService.Delete(thumbFilePath!, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(thumbHalfFilePath))
        {
            await storageService.Delete(thumbHalfFilePath!, cancellationToken);
        }
    }
}

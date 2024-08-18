using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias;
public class UploadMedias(IVideoRepository videoRepository,
    IUnitOfWork unitOfWork,
    IStorageService storageService) : IUploadMedias
{
    public async Task Handle(UploadMediasInput request, CancellationToken cancellationToken)
    {
        var video = await videoRepository.Get(request.VideoId, cancellationToken);
        try
        {
            await UploadVideo(storageService, request, video, cancellationToken);
            await UploadTrailer(storageService, request, video, cancellationToken);
            await videoRepository.Update(video, cancellationToken);
            await unitOfWork.Commit(cancellationToken);
        }
        catch (Exception)
        {
            await ClearStorage(storageService, request, video, cancellationToken);
            throw;
        }
    }

    private static async Task ClearStorage(IStorageService storageService, UploadMediasInput request, Domain.Entity.Video video, CancellationToken cancellationToken)
    {
        if (request.VideoInput is not null && video.Media is not null)
        {
            await storageService.Delete(video.Media.FilePath, cancellationToken);
        }
        if (request.TrailerInput is not null && video.Trailer is not null)
        {
            await storageService.Delete(video.Trailer.FilePath, cancellationToken);
        }
    }

    private static async Task UploadTrailer(IStorageService storageService, UploadMediasInput request, Domain.Entity.Video video, CancellationToken cancellationToken)
    {
        if (request.TrailerInput is not null)
        {
            var uploadedFilePath = await storageService.Upload(
                StorageName.Create(video.Id, nameof(video.Trailer), 
                request.TrailerInput.Extension),
                request.TrailerInput.FileStream,
                request.TrailerInput.ContentType,
                cancellationToken);
            video.UpdateTrailer(uploadedFilePath);
        }
    }

    private static async Task UploadVideo(IStorageService storageService, UploadMediasInput request, Domain.Entity.Video video, CancellationToken cancellationToken)
    {
        if (request.VideoInput is not null)
        {
            var uploadedFilePath = await storageService.Upload(
                StorageName.Create(video.Id, nameof(video.Media), request.VideoInput.Extension),
                request.VideoInput.FileStream,
                request.VideoInput.ContentType,
                cancellationToken);
            video.UpdateMedia(uploadedFilePath);
        }
    }
}

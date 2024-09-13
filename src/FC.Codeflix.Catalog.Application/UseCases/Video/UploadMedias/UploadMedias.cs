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
            await UploadImages(storageService, request, video, cancellationToken);
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
        if (request.Banner is not null && video.Banner is not null)
        {
            await storageService.Delete(video.Banner.Path, cancellationToken);
        }
        if (request.Thumb is not null && video.Thumb is not null)
        {
            await storageService.Delete(video.Thumb.Path, cancellationToken);
        }
        if (request.ThumbHalf is not null && video.ThumbHalf is not null)
        {
            await storageService.Delete(video.ThumbHalf.Path, cancellationToken);
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

    private static async Task UploadImages(IStorageService storageService, 
        UploadMediasInput request, 
        Domain.Entity.Video video, CancellationToken cancellationToken)
    {
        if (request.Banner is not null)
        {
            var uploadedFilePath = await storageService.Upload(
                StorageName.Create(video.Id, nameof(video.Banner),
                request.Banner.Extension),
                request.Banner.FileStream,
                request.Banner.ContentType,
                cancellationToken);
            video.UpdateBanner(uploadedFilePath);
        }

        if (request.Thumb is not null)
        {
            var uploadedFilePath = await storageService.Upload(
                StorageName.Create(video.Id, nameof(video.Thumb),
                request.Thumb.Extension),
                request.Thumb.FileStream,
                request.Thumb.ContentType,
                cancellationToken);
            video.UpdateThumb(uploadedFilePath);
        }

        if (request.ThumbHalf is not null)
        {
            var uploadedFilePath = await storageService.Upload(
                StorageName.Create(video.Id, nameof(video.ThumbHalf),
                request.ThumbHalf.Extension),
                request.ThumbHalf.FileStream,
                request.ThumbHalf.ContentType,
                cancellationToken);
            video.UpdateThumbHalf(uploadedFilePath);
        }
    }
}

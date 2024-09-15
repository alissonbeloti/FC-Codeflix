using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UpdateMediaStatus;
public class UpdateMediaStatus(IVideoRepository videoRepository, IUnitOfWork unitOfWork,
    ILogger<UpdateMediaStatus> logger) : IUpdateMediaStatus
{
    
    public async Task<VideoModelOutput> Handle(UpdateMediaStatusInput request, CancellationToken cancellationToken)
    {
        var video = await videoRepository.Get(request.VideoId, cancellationToken);

        switch (request.Status)
        {
            case Domain.Enum.MediaStatus.Completed:
                video.UpdateAsEncoded(request.EncodedPath!);
                break;
            case Domain.Enum.MediaStatus.Error:
                logger.LogError("There was an error while trying to encode the video {videoId}: {error}", 
                    video.Id, request.ErrorMessage
                    );
                video.UpdateAsEncodingError();
                break;
            default:
                throw new EntityValidationException("Invalid media status");
        }

        await videoRepository.Update(video, cancellationToken);
        await unitOfWork.Commit(cancellationToken);

        return VideoModelOutput.FromVideo(video);
    }
}

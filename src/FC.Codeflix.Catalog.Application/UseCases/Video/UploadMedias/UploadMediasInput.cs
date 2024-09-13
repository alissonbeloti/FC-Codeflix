using FC.Codeflix.Catalog.Application.UseCases.Video.Common;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias;
public record UploadMediasInput(
    Guid VideoId, 
    FileInput? VideoInput = null, 
    FileInput? TrailerInput = null,
    FileInput? Banner = null,
    FileInput? Thumb = null,
    FileInput? ThumbHalf = null
    ) : IRequest;

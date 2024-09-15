using MediatR;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UpdateMediaStatus;
public record UpdateMediaStatusInput(
    Guid VideoId,
    MediaStatus Status,
    string? EncodedPath = null,
    string? ErrorMessage = null
    ) : IRequest<VideoModelOutput>;

using MediatR;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;
public record UpdateVideoInput(
    Guid VideoId,
    string Title,
    string Description,
    Rating Rating,
    int YearLaunched,
    bool Published,
    int Duration,
    bool Opened,
    IReadOnlyCollection<Guid>? CategoriesIds = null,
    IReadOnlyCollection<Guid>? GenresIds = null,
    IReadOnlyCollection<Guid>? CastMembersIds = null,
    FileInput? Thumb = null,
    FileInput? Banner = null,
    FileInput? ThumbHalf = null
    ) 
    : IRequest<VideoModelOutput>;
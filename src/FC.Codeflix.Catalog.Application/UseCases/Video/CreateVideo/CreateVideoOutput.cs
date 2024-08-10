using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Enum;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
public record CreateVideoOutput(
    Guid Id,
    DateTime CreatedAt,
    string Title,
    string Description,
    Rating Rating,
    int YearLaunched,
    int Duration,
    bool Published,
    bool Opened,
    IReadOnlyCollection<Guid>? CategoriesIds = null,
    IReadOnlyCollection<Guid>? GenresIds = null,
    IReadOnlyCollection<Guid>? CastMembersIds = null,
    string? Thumb = null,
    string? Banner = null,
    string? ThumbHalf = null
    )
{
    public static CreateVideoOutput FromVideo(Domain.Entity.Video video) =>
        new(video.Id,
            video.CreatedAt, video.Title, video.Description, video.Rating,
            video.YearLaunched, video.Duration, video.Published, video.Opened,
            video.Categories,
            video.Genres,
            video.CastMembers,
            video.Thumb?.Path,
            video.Banner?.Path,
            video.ThumbHalf?.Path);
}

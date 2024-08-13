using FC.Codeflix.Catalog.Domain.Extensions;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.Common;
public record VideoModelOutput(Guid Id,
    DateTime CreatedAt,
    string Title,
    string Description,
    string Rating,
    int YearLaunched,
    int Duration,
    bool Published,
    bool Opened,
    IReadOnlyCollection<VideoModelOutputRelatedAggregate>? Categories = null,
    IReadOnlyCollection<VideoModelOutputRelatedAggregate>? Genres = null,
    IReadOnlyCollection<VideoModelOutputRelatedAggregate>? CastMembers = null,
    string? ThumbFileUrl = null,
    string? BannerFileUrl = null,
    string? ThumbHalfFileUrl = null,
    string? VideoFileUrl = null,
    string? TrailerFileUrl = null
    )
{
    public static VideoModelOutput FromVideo(Domain.Entity.Video video) =>
        new(video.Id,
            video.CreatedAt, 
            video.Title, 
            video.Description, 
            video.Rating.ToStringSignal(),
            video.YearLaunched, 
            video.Duration, 
            video.Published, 
            video.Opened,
            video.Categories.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList(),
            video.Genres.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList(),
            video.CastMembers.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList(),
            video.Thumb?.Path,
            video.Banner?.Path,
            video.ThumbHalf?.Path,
            video.Media?.FilePath,
            video.Trailer?.FilePath);
    public static VideoModelOutput FromVideo(Domain.Entity.Video video,
        List<Domain.Entity.Category>? categories = null,
        List<Domain.Entity.Genre>? genres = null,
        List<Domain.Entity.CastMember>? castMembers = null
        ) =>
        new(video.Id,
            video.CreatedAt,
            video.Title,
            video.Description,
            video.Rating.ToStringSignal(),
            video.YearLaunched,
            video.Duration,
            video.Published,
            video.Opened,
            video.Categories.Select(id => new VideoModelOutputRelatedAggregate(
                id,
                categories?.FirstOrDefault(category => category.Id == id)?.Name))
            .ToList(),
            video.Genres.Select(id => new VideoModelOutputRelatedAggregate(
                id,
                genres?.FirstOrDefault(genre => genre.Id == id)?.Name))
            .ToList(),
            video.CastMembers.Select(id => new VideoModelOutputRelatedAggregate(
                id,
                castMembers?.FirstOrDefault(castMember => castMember.Id == id)?.Name))
            .ToList(),
            video.Thumb?.Path,
            video.Banner?.Path,
            video.ThumbHalf?.Path,
            video.Media?.FilePath,
            video.Trailer?.FilePath);
}

public record VideoModelOutputRelatedAggregate(Guid Id, string? Name = null);
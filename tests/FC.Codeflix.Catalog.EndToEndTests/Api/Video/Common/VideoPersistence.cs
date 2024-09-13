using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
public class VideoPersistence(CodeflixCatalogDbContext context)
{
    public async Task<DomainEntity.Video?> GetById(Guid id)
        =>  await context.Videos.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<List<VideosCastMembers>?> GetVideosCastMembers(Guid videoId)
        => await context.VideosCastMembers.AsNoTracking()
            .Where(x => x.VideoId == videoId).ToListAsync();

    public async Task<List<VideosCategories>?> GetVideosCategories(Guid videoId)
        => await context.VideosCategories.AsNoTracking()
            .Where(x => x.VideoId == videoId).ToListAsync();
    public async Task<List<VideosGenres>?> GetVideosGenres(Guid videoId)
        => await context.VideosGenres.AsNoTracking()
            .Where(x => x.VideoId == videoId).ToListAsync();

    public async Task InsertList(List<DomainEntity.Video> videos)
    {
        await context.Videos.AddRangeAsync(videos);
        foreach (var video in videos)
        {
            var videoCategories = video.Categories
                .Select(categoryId => new VideosCategories(categoryId, video.Id));
            if (videoCategories is not null && videoCategories.Any())
            {
                await context.VideosCategories.AddRangeAsync(videoCategories);
            }

            var videoGenres = video.Genres
                .Select(id => new VideosGenres(id, video.Id));
            if (videoGenres is not null && videoGenres.Any())
            {
                await context.VideosGenres.AddRangeAsync(videoGenres);
            }

            var videoCastMembers = video.CastMembers
                .Select(id => new VideosCastMembers(id, video.Id));
            if (videoCastMembers is not null && videoCastMembers.Any())
            {
                await context.VideosCastMembers.AddRangeAsync(videoCastMembers);
            }
        }
        await context.SaveChangesAsync();
    }

    public async Task<int> GetMediaCount()
        => await context.Set<Media>().CountAsync();
}

using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
public class VideoRepository : IVideoRepository
{
    private readonly CodeflixCatalogDbContext _context;
    private DbSet<Video> _videos => _context.Set<Video>();
    private DbSet<VideosCategories> _videosCategories => _context.Set<VideosCategories>();
    private DbSet<VideosCastMembers> _videosCastMembers => _context.Set<VideosCastMembers>();
    private DbSet<VideosGenres> _videosGenres => _context.Set<VideosGenres>();
    private DbSet<Media> _medias => _context.Set<Media>();

    public VideoRepository(CodeflixCatalogDbContext context)
    {
        _context = context;
    }

    public Task Delete(Video aggregate, CancellationToken cancellationToken)
    {
        _videosCategories.RemoveRange(_videosCategories
            .Where(x => x.VideoId == aggregate.Id));
        _videosCastMembers.RemoveRange(_videosCastMembers
            .Where(x => x.VideoId == aggregate.Id));
        _videosGenres.RemoveRange(_videosGenres
            .Where(x => x.VideoId == aggregate.Id));

        if (aggregate.Trailer is not null)
            _medias.Remove(aggregate.Trailer);
        if (aggregate.Media is not null)
            _medias.Remove(aggregate.Media);

        _videos.Remove(aggregate);
        return Task.CompletedTask;
    }

    public async Task<Video> Get(Guid id, CancellationToken cancellationToken)
    {
        var video = await _videos.FirstOrDefaultAsync(x => x.Id == id);
        NotFoundException.ThrowIfNull(video, $"Video '{id}' not found.");
        await PopulateRelations(video, cancellationToken);

        return video;
    }

    public async Task Insert(Video video, CancellationToken cancellationToken)
    {
        await _videos.AddAsync(video, cancellationToken);
        await AddRelations(video, cancellationToken);
    }

    public async Task<SearchOutput<Video>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        var toSkip = (input.Page - 1) * input.PerPage;
        var query = _videos.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(input.Search))
            query = query.Where(x => x.Title.Contains(input.Search));
        query = AddOrderBy(input, query);

        var count = await query.CountAsync();
        var items = await query.Skip(toSkip).Take(input.PerPage).ToListAsync();
        foreach (var video in items)
        {
            await PopulateRelations(video, cancellationToken);
        }

        return new(input.Page, input.PerPage, count, items.AsReadOnly());
    }

    public async Task Update(Video aggregate, CancellationToken cancellationToken)
    {
        await AddRelations(aggregate, cancellationToken, true);
        DeleteOrphanMedias(aggregate);
        _videos.Update(aggregate);
    }

    private void DeleteOrphanMedias(Video video)
    {
        if (_context.Entry(video).Reference(v => v.Trailer).IsModified)
        {
            var oldTrailerId = _context.Entry(video)
                .OriginalValues.GetValue<Guid?>($"{nameof(Video.Trailer)}Id");
            if (oldTrailerId is not null && oldTrailerId != video.Trailer?.Id)
            {
                var oldTrailer = _medias.Find(oldTrailerId);
                _medias.Remove(oldTrailer!);
            }
        }

        if (_context.Entry(video).Reference(v => v.Media).IsModified)
        {
            var oldMediaId = _context.Entry(video)
                .OriginalValues.GetValue<Guid?>($"{nameof(Video.Media)}Id");
            if (oldMediaId is not null && oldMediaId != video.Media?.Id)
            {
                var oldMedia = _medias.Find(oldMediaId);
                _medias.Remove(oldMedia!);
            }
        }
    }
    private async Task AddRelations(Video video, CancellationToken cancellationToken, bool update = false)
    {
        if (video.Categories.Any())
        {
            if (update)
                _videosCategories.RemoveRange(_videosCategories.
                    Where(x => x.VideoId == video.Id));
            var relations = video.Categories.Select(
                categoryId => new VideosCategories(categoryId, video.Id)
            );
            await _videosCategories.AddRangeAsync(relations, cancellationToken);
        }
        if (video.Genres.Any())
        {
            if (update)
                _videosGenres.RemoveRange(_videosGenres.
                    Where(x => x.VideoId == video.Id));
            var relations = video.Genres.Select(
                genreId => new VideosGenres(genreId, video.Id)
            );
            await _videosGenres.AddRangeAsync(relations, cancellationToken);
        }
        if (video.CastMembers.Any())
        {
            if (update)
                _videosCastMembers.RemoveRange(_videosCastMembers.
                    Where(x => x.VideoId == video.Id));
            var relations = video.CastMembers.Select(
                id => new VideosCastMembers(id, video.Id)
            );
            await _videosCastMembers.AddRangeAsync(relations, cancellationToken);
        }
    }

    private static IQueryable<Video> AddOrderBy(SearchInput input, IQueryable<Video> query)
    {
        input.OrderBy = input.OrderBy.ToLower();
        query = input switch
        {
            { OrderBy: "title", Order: SearchOrder.Asc } =>
                query.OrderBy(video => video.Title).ThenBy(v => v.Id),
            { OrderBy: "title", Order: SearchOrder.Desc } =>
                query.OrderByDescending(video => video.Title).ThenBy(v => v.Id),
            { OrderBy: "id", Order: SearchOrder.Asc } =>
                query.OrderBy(video => video.Id),
            { OrderBy: "id", Order: SearchOrder.Desc } =>
                query.OrderByDescending(video => video.Id),
            { OrderBy: "createdat", Order: SearchOrder.Asc } =>
                query.OrderBy(video => video.CreatedAt).ThenBy(v => v.Id),
            { OrderBy: "createdat", Order: SearchOrder.Desc } =>
                query.OrderByDescending(video => video.CreatedAt).ThenBy(v => v.Id),
            _ => query.OrderBy(v => v.Title).ThenBy(v => v.Id),
        };
        return query;
    }

    private async Task PopulateRelations(Video video, CancellationToken cancellationToken)
    {
        var categoryIds = await _videosCategories.AsNoTracking()
                            .Where(x => x.VideoId == video!.Id)
                            .Select(x => x.CategoryId).ToListAsync(cancellationToken);
        categoryIds.ForEach(video!.AddCategory);
        var genresIds = await _videosGenres.AsNoTracking()
                    .Where(x => x.VideoId == video!.Id)
                    .Select(x => x.GenreId).ToListAsync(cancellationToken);
        genresIds.ForEach(video!.AddGenre);

        var castMembersIds = await _videosCastMembers.AsNoTracking()
                    .Where(x => x.VideoId == video!.Id)
                    .Select(x => x.CastMemberId).ToListAsync(cancellationToken);
        castMembersIds.ForEach(video!.AddCastMember);
    }
}

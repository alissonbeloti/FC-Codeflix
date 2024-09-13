using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;

using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly CodeflixCatalogDbContext _context;
        private DbSet<Genre> _genres => _context.Set<Genre>();
        private DbSet<GenresCategories> _genresCategories => _context.Set<GenresCategories>();

        public GenreRepository(CodeflixCatalogDbContext context)
            => _context = context;

        public async Task Insert(Genre aggregate, CancellationToken cancellationToken)
        {
            await _genres.AddAsync(aggregate, cancellationToken);
            await AddCategories(aggregate, cancellationToken);
        }


        public async Task Update(Genre aggregate, CancellationToken cancellationToken)
        {
            _genresCategories.RemoveRange(_genresCategories.Where(r => r.GenreId == aggregate.Id));
            _genres.Update(aggregate);
            await AddCategories(aggregate, cancellationToken);
        }

        public Task Delete(Genre aggregate, CancellationToken cancellationToken)
        {
            _genresCategories.RemoveRange(
                _genresCategories.Where(x => x.GenreId == aggregate.Id)
                );
            _genres.Remove(aggregate);
            return Task.CompletedTask;
        }

        public async Task<Genre> Get(Guid id, CancellationToken cancellationToken)
        {
            var genre = await _genres.AsNoTracking().SingleOrDefaultAsync(g => g.Id == id, cancellationToken);
            NotFoundException.ThrowIfNull(genre, $"Genre '{id}' not found.");
            var categoryIds = await _genresCategories.AsNoTracking()
                .Where(x => x.GenreId == genre.Id)
                .Select(x => x.CategoryId).ToListAsync(cancellationToken);
            categoryIds.ForEach(genre.AddCategory);
            return genre;
        }

        public async Task<SearchOutput<Genre>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
        {
            var toSkip = (input.Page - 1) * input.PerPage;
            var query = _genres.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(input.Search))
            {
                query = query.Where(genre => genre.Name.Contains(input.Search));
            }
            query = AddOrderToQuery(query, input.OrderBy, input.Order);

            var total = await query.CountAsync();

            var genres = await query
                .Skip(toSkip).Take(input.PerPage).ToListAsync();
            var genresIds = genres.Select(genre => genre.Id).ToList();
            var relations = await _genresCategories
                .Where(relation => genresIds.Contains(relation.GenreId))
                .ToListAsync(cancellationToken);
            var relationsByGenreIdGroup = relations.GroupBy(x => x.GenreId).ToList();
            relationsByGenreIdGroup.ForEach(relationGroup =>
            {
                var genre = genres.Find(genre => genre.Id == relationGroup.Key);
                if (genre is null) return;
                relationGroup.ToList().ForEach(relation =>
                {
                    genre.AddCategory(relation.CategoryId);
                });
            });
            return new SearchOutput<Genre>(input.Page, input.PerPage, total, genres);
        }
        private async Task AddCategories(Genre aggregate, CancellationToken cancellationToken)
        {
            if (aggregate.Categories.Any())
            {
                var relations = aggregate.Categories.Select(
                    categoryId => new GenresCategories(categoryId, aggregate.Id))
                    ;
                await _genresCategories.AddRangeAsync(relations, cancellationToken);
            }
        }

        private IQueryable<Genre> AddOrderToQuery(
        IQueryable<Genre> query,
        string orderProperty,
        SearchOrder order
        )
        {
            var orderedQuery = (orderProperty.ToLower(), order) switch
            {
                ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name)
                    .ThenBy(x => x.Id),
                ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name)
                    .ThenByDescending(x => x.Id),
                ("id", SearchOrder.Asc) => query.OrderBy(x => x.Id),
                ("id", SearchOrder.Desc) => query.OrderByDescending(x => x.Id),
                ("createdat", SearchOrder.Asc) => query.OrderBy(x => x.CreatedAt),
                ("createdat", SearchOrder.Desc) => query.OrderByDescending(x => x.CreatedAt),
                _ => query.OrderBy(x => x.Name)
                    .ThenBy(x => x.Id)
            };
            return orderedQuery;
        }

        public async Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> ids, CancellationToken cancellationToken)
        => (await _genres.AsNoTracking().Where(genre => ids.ToArray().Contains(genre.Id))
            .Select(genre => genre.Id)
            .ToListAsync(cancellationToken))
            .AsReadOnly();

        public async Task<IReadOnlyList<Genre>> GetListByIds(List<Guid> ids, CancellationToken cancellationToken)
       => (await _genres.AsNoTracking().Where(genre => ids.ToArray().Contains(genre.Id))
            .ToListAsync(cancellationToken))
            .AsReadOnly();
    }
}

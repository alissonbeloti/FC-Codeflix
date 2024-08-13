using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;

using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories
{
    public class CastMemberRepository(CodeflixCatalogDbContext context) 
        : ICastMemberRepository
    {
        private readonly DbSet<CastMember> castMembers = context.Set<CastMember>();
        public Task Delete(CastMember aggregate, CancellationToken cancellationToken)
        {
            castMembers.Remove(aggregate);
            return Task.CompletedTask;
        }

        public async Task<CastMember> Get(Guid id, CancellationToken cancellationToken)
        { 
            var castMember = await castMembers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            NotFoundException.ThrowIfNull(castMember, $"CastMember '{id}' not found.");
            return castMember!;
        }

        public async Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> ids, CancellationToken cancellationToken)
         => (await castMembers.AsNoTracking().Where(castmember => ids.Contains(castmember.Id))
            .Select(cast => cast.Id)
            .ToListAsync(cancellationToken))
            .AsReadOnly();

        public Task<IReadOnlyList<CastMember>> GetListByIds(List<Guid> ids, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task Insert(CastMember aggregate, CancellationToken cancellationToken) 
            => await castMembers.AddAsync(aggregate, cancellationToken);

        public async Task<SearchOutput<CastMember>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
        {
            var toSkip = (input.Page -1) * input.PerPage;
            var query = castMembers.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(input.Search))
                query = query.Where(x => x.Name.Contains(input.Search));
            query = AddOrderToQuery(query, input.OrderBy, input.Order);

            var count = await query.CountAsync();
            var items = await query
                .Skip(toSkip)
                .Take(input.PerPage)
                .ToListAsync();

            return new SearchOutput<CastMember>(
                input.Page, 
                input.PerPage, 
                count, 
                items.AsReadOnly());
        }

        public Task Update(CastMember aggregate, CancellationToken cancellationToken)
        {
            castMembers.Update(aggregate);
            return Task.CompletedTask;
        }

        private IQueryable<CastMember> AddOrderToQuery(
            IQueryable<CastMember> query,
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
    }
}

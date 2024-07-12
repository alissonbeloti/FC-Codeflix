using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;

using Microsoft.EntityFrameworkCore;

using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
public class CategoryPersitence
{
    private readonly CodeflixCatalogDbContext _context;

    public CategoryPersitence(CodeflixCatalogDbContext context)
    {
        _context = context;
    }

    public Task<DomainEntity.Category?> GetById(Guid id)
        => _context
            .Categories.AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == id);

    public async Task InsertList(List<DomainEntity.Category> entities)
    {
        await _context.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }
}

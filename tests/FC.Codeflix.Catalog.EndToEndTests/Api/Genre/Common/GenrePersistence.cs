using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
public class GenrePersistence
{
    private readonly CodeflixCatalogDbContext _context;
    public GenrePersistence(CodeflixCatalogDbContext context) => _context = context;

    public Task<DomainEntity.Genre?> GetById(Guid id)
        => _context
            .Genres.AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == id);

    public async Task InsertList(List<DomainEntity.Genre> genres)
    {
        await _context.AddRangeAsync(genres);
        await _context.SaveChangesAsync();
    }

    public async Task InsertGenresCategoriesRelationsList(List<GenresCategories> relations)
    {
        await _context.GenresCategories.AddRangeAsync(relations);
        await _context.SaveChangesAsync();
    }

    internal async Task<List<GenresCategories>> GetGenresCategoriesRelationsByGenreId(Guid id)
        => await _context.GenresCategories.AsNoTracking().Where(relation =>
            relation.GenreId == id).ToListAsync();
}

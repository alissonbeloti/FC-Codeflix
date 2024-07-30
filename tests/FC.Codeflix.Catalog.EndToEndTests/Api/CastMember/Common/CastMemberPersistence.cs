using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.EndToEndTests.Base;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;
public class CastMemberPersistence(CodeflixCatalogDbContext context) : BaseFixture
{

    public async Task<DomainEntity.CastMember?> GetById(Guid id)
        => await context
            .CastMembers.AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == id);

    public async Task InsertList(List<DomainEntity.CastMember> castMembers)
    {
        await context.AddRangeAsync(castMembers);
        await context.SaveChangesAsync();
    }

     
}

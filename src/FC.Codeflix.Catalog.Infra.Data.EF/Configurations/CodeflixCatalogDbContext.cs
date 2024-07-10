using FC.Codeflix.Catalog.Domain.Entity;

using Microsoft.EntityFrameworkCore;

using System.Security.Cryptography.X509Certificates;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
public class CodeflixCatalogDbContext: DbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public CodeflixCatalogDbContext(DbContextOptions<CodeflixCatalogDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}

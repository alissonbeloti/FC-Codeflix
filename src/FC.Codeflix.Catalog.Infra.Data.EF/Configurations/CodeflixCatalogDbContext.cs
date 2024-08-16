using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;

using Microsoft.EntityFrameworkCore;

using System.Security.Cryptography.X509Certificates;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
public class CodeflixCatalogDbContext: DbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<GenresCategories> GenresCategories => Set<GenresCategories>();

    public DbSet<CastMember> CastMembers => Set<CastMember>();
    public DbSet<Video> Videos => Set<Video>();
    public DbSet<VideosCategories> VideosCategories => Set<VideosCategories>();
    public DbSet<VideosCastMembers> VideosCastMembers => Set<VideosCastMembers>();
    public DbSet<VideosGenres> VideosGenres => Set<VideosGenres>();
    public CodeflixCatalogDbContext(DbContextOptions<CodeflixCatalogDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new GenreConfiguration());
        modelBuilder.ApplyConfiguration(new GenresCategoriesConfiguration());
        modelBuilder.ApplyConfiguration(new VideoConfiguration());
        modelBuilder.ApplyConfiguration(new VideosCategoriesConfiguration());
        modelBuilder.ApplyConfiguration(new VideosGenresConfiguration());
        modelBuilder.ApplyConfiguration(new VideosCastMembersConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}

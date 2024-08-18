using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
internal class GenreConfiguration
    : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired();
        builder.Ignore(c => c.Events);
    }
}

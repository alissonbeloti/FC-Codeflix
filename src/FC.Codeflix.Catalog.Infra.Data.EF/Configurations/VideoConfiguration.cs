using FC.Codeflix.Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
internal class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).ValueGeneratedNever();
        builder.Property(v => v.Title).HasMaxLength(255);
        builder.Property(v => v.Description).HasMaxLength(4_000);
        //AutoInclude
        builder.Navigation(x => x.Media).AutoInclude();
        builder.Navigation(x => x.Trailer).AutoInclude();
        //value objects
        builder.OwnsOne(video => video.Thumb, thumb =>
            thumb.Property(image => image.Path).HasColumnName("ThumbPath")
        );
        builder.OwnsOne(video => video.Banner, banner =>
            banner.Property(image => image.Path).HasColumnName("BannerPath")
        );
        builder.OwnsOne(video => video.ThumbHalf, thumbHalf =>
            thumbHalf.Property(image => image.Path).HasColumnName("ThumbHalfPath")
        );
        //relations with others entities
        builder.HasOne(x => x.Media).WithOne().HasForeignKey<Media>();
        builder.HasOne(x => x.Trailer).WithOne().HasForeignKey<Media>();
        
    }
}

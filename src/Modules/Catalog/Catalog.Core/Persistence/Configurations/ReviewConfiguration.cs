using Catalog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Core.Persistence.Configurations;

internal class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .ValueGeneratedOnAdd();

        builder.Property(r => r.Content)
            .IsRequired()
            .HasMaxLength(1000);

        builder.OwnsOne(r => r.Rating, rating =>
        {
            rating.Property(r => r.Value)
                .IsRequired()
                .HasColumnName("Rating");
        });
    }
}

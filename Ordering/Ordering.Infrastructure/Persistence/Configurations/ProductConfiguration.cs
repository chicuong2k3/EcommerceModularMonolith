using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.ProductAggregate;

namespace Ordering.Infrastructure.Persistence.Configurations;

internal class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();


        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.ImageUrl)
               .HasMaxLength(2000);

        builder.Property(p => p.AttributesDescription)
            .HasMaxLength(250);
    }
}

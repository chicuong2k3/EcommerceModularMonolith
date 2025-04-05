using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.ProductAggregate;

namespace Ordering.Infrastructure.Persistence.Configurations;

internal class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.HasKey(pv => pv.Id);

        builder.Property(pv => pv.Id)
            .ValueGeneratedNever();

        builder.Property(pv => pv.ImageUrl)
               .HasMaxLength(2000);

        builder.Property(pv => pv.AttributesDescription)
            .HasMaxLength(250);
    }
}

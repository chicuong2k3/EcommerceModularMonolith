using Catalog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

internal sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.HasKey(pv => pv.Id);

        builder.Property(pv => pv.Id)
            .ValueGeneratedNever();

        builder.OwnsOne(pv => pv.OriginalPrice, price =>
        {
            price.Property(p => p.Amount)
                .HasColumnName("OriginalPrice")
                .IsRequired();

        });

        builder.OwnsOne(pv => pv.Image, image =>
        {
            image.Property(i => i.Url)
                .HasMaxLength(2000);

            image.Property(i => i.AltText)
                .HasMaxLength(100);
        });

        builder.OwnsOne(pv => pv.SalePrice, salePrice =>
        {
            salePrice.Property(d => d.Amount)
                .HasColumnName("SalePrice");
        });

        builder.OwnsOne(d => d.SalePriceEffectivePeriod, period =>
        {
            period.Property(p => p.Start)
                .HasColumnName("DiscountStart");

            period.Property(p => p.End)
                .HasColumnName("DiscountEnd");
        });

        builder.HasMany(pv => pv.Attributes)
            .WithOne()
            .HasForeignKey("ProductVariantId");
    }
}

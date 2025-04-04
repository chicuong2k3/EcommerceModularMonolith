using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ordering.Infrastructure.Persistence.Configurations;

internal sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ProductId)
            .IsRequired();
        builder.Property(x => x.ProductVariantId)
            .IsRequired();

        builder.OwnsOne(x => x.OriginalPrice, originalPrice =>
        {
            originalPrice.Property(x => x.Amount)
                .HasColumnName("OriginalPrice")
                .IsRequired();
        });

        builder.OwnsOne(x => x.SalePrice, salePrice =>
        {
            salePrice.Property(x => x.Amount)
                .HasColumnName("SalePrice")
                .IsRequired();
        });
    }
}

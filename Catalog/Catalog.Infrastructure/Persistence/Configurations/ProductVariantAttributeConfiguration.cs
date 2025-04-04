using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence;

internal sealed class ProductVariantAttributeConfiguration : IEntityTypeConfiguration<ProductVariantAttribute>
{
    public void Configure(EntityTypeBuilder<ProductVariantAttribute> builder)
    {
        builder.HasKey(["Value", "AttributeId", "ProductVariantId"]);

        builder.Property(pva => pva.Value)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne<ProductAttribute>()
            .WithMany()
            .HasForeignKey("AttributeId");

    }
}

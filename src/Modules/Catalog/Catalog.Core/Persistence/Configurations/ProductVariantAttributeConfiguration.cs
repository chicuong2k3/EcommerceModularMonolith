using Catalog.Core.Entities;
using Catalog.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Core.Persistence.Configurations;

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

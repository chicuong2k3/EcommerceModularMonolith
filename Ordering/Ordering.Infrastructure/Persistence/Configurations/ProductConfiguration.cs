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

        builder.HasMany(p => p.Variants)
            .WithOne()
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

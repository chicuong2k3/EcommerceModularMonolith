using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Promotion.Infrastructure.Persistence.Configurations;

internal sealed class PercentageDiscountConfiguration : IEntityTypeConfiguration<PercentageDiscount>
{
    public void Configure(EntityTypeBuilder<PercentageDiscount> builder)
    {
        builder.Property(d => d.Percentage)
            .HasColumnName("Percentage");
    }
}

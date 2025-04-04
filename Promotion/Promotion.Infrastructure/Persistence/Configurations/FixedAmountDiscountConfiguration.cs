using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Promotion.Infrastructure.Persistence.Configurations;

internal sealed class FixedAmountDiscountConfiguration : IEntityTypeConfiguration<FixedAmountDiscount>
{
    public void Configure(EntityTypeBuilder<FixedAmountDiscount> builder)
    {
        builder.OwnsOne(d => d.FixedAmount, m =>
        {
            m.Property(p => p.Amount)
                .HasColumnName("FixedAmount");
        });
    }
}

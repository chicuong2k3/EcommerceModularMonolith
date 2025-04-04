using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Promotion.Infrastructure.Persistence.Configurations;

internal sealed class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .ValueGeneratedNever();


        builder.HasDiscriminator<string>("DiscountType")
            .HasValue<FixedAmountDiscount>("FixedAmount")
            .HasValue<PercentageDiscount>("Percentage");
    }
}

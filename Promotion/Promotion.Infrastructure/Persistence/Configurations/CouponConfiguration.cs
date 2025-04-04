using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Promotion.Infrastructure.Persistence.Configurations;

internal sealed class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.ExpiryDate)
            .IsRequired();

        builder.Property(c => c.UsageLimit)
            .IsRequired();

        builder.Property(c => c.CurrentUsageCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasOne(c => c.Discount)
            .WithMany()
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(c => c.Conditions)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "CouponCondition",
                j => j.HasOne<Condition>().WithMany().HasForeignKey("ConditionId"),
                j => j.HasOne<Coupon>().WithMany().HasForeignKey("CouponId"));
    }
}

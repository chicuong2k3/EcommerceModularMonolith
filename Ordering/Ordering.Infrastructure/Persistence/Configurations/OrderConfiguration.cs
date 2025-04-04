using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ordering.Infrastructure.Persistence.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .ValueGeneratedNever();

        builder.Property(o => o.Status)
            .HasConversion(
                o => o.ToString(),
                o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o)
            );

        builder.OwnsOne(o => o.ShippingInfo, shipping =>
        {
            shipping.OwnsOne(s => s.ShippingCosts, costs =>
            {
                costs.Property(c => c.Amount)
                    .IsRequired()
                    .HasColumnName("ShippingCosts");
            });

            shipping.OwnsOne(s => s.ShippingAddress, addr =>
            {
                addr.Property(a => a.Street)
                    .HasMaxLength(100)
                    .HasColumnName("Street");

                addr.Property(a => a.Ward)
                    .HasMaxLength(50)
                    .HasColumnName("Ward");

                addr.Property(a => a.District)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("District");

                addr.Property(a => a.Province)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("Province");

                addr.Property(a => a.Country)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("Country");
            });
        });

        builder.OwnsOne(o => o.PaymentInfo, payment =>
        {
            payment.Property(p => p.PaymentMethod)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("PaymentMethod");
        });

        builder.OwnsOne(o => o.Total, total =>
        {
            total.Property(t => t.Amount)
                .IsRequired()
                .HasColumnName("Total");
        });

        builder.OwnsOne(o => o.Subtotal, subtotal =>
        {
            subtotal.Property(t => t.Amount)
                .IsRequired()
                .HasColumnName("Subtotal");
        });

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

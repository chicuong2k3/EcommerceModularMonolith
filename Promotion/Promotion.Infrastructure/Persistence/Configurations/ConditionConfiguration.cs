using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Promotion.Infrastructure.Persistence.Configurations;

internal sealed class ConditionConfiguration : IEntityTypeConfiguration<Condition>
{
    public void Configure(EntityTypeBuilder<Condition> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Type)
            .HasMaxLength(50)
            .IsRequired()
            .HasConversion(builder => builder.ToString(), value => (ConditionType)Enum.Parse(typeof(ConditionType), value));

        builder.Property(c => c.Value)
            .IsRequired();
    }
}

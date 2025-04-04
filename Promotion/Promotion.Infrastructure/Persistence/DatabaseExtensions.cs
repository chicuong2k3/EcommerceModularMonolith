using Microsoft.Extensions.DependencyInjection;

namespace Promotion.Infrastructure.Persistence;

public static class DatabaseExtensions
{
    public static void MigratePromotionDatabase(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<PromotionDbContext>();

        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }

        var conditions = new List<Condition>()
            {
                Condition.Create("min200k", ConditionType.MinOrderTotal, "200000").Value,
                Condition.Create("min50k", ConditionType.MinOrderTotal, "50000").Value,
                Condition.Create("min100k", ConditionType.MinOrderTotal, "100000").Value
            };

        if (!context.Conditions.Any())
        {
            context.Conditions.AddRange(conditions);
            context.SaveChanges();
        }

        var discountService = services.GetRequiredService<IDiscountService>();

        var coupons = new List<Coupon>()
        {
            Coupon.CreateAsync(discountService, "SALE20K", "FixedAmount", 20000, DateTime.UtcNow.AddHours(1), 10, "", [conditions[1]]).Result.Value,
            Coupon.CreateAsync(discountService, "SALE50K", "FixedAmount", 50000, DateTime.UtcNow.AddHours(2), 20, "", [conditions[2]]).Result.Value,
            Coupon.CreateAsync(discountService, "SALE30%", "Percentage", 30, DateTime.UtcNow.AddHours(3), 30, "").Result.Value
        };

        if (!context.Coupons.Any())
        {
            context.Coupons.AddRange(coupons);
            context.SaveChanges();
        }
    }
}

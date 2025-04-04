
using Microsoft.EntityFrameworkCore;
using Promotion.Infrastructure.Persistence;

namespace Promotion.Infrastructure.Services;

internal class DiscountService : IDiscountService
{
    private readonly PromotionDbContext dbContext;

    public DiscountService(PromotionDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Discount?> GetByTypeAndValueAsync(string discountType, decimal discountValue)
    {
        discountType = discountType.ToLowerInvariant();
        Discount? discount = null;
        if (discountType == "fixedamount")
        {
            discount = await dbContext.Discounts
                .OfType<FixedAmountDiscount>()
                .FirstOrDefaultAsync(d => d.FixedAmount.Amount == discountValue);
        }
        else if (discountType == "percentage")
        {
            discount = await dbContext.Discounts
                .OfType<PercentageDiscount>()
                .FirstOrDefaultAsync(d => (decimal)d.Percentage == discountValue);
        }
        else
        {
            throw new ArgumentException($"Unknown discount type: {discountType}", nameof(discountType));
        }

        return discount;
    }
}

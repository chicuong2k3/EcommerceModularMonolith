namespace Promotion.Domain.CouponAggregate;

public class FixedAmountDiscount : Discount
{
    public Money FixedAmount { get; }

    private FixedAmountDiscount()
    {
    }

    public FixedAmountDiscount(Money fixedAmount)
    {
        Id = Guid.NewGuid();
        FixedAmount = fixedAmount;
    }

    public override Money CalculateDiscountAmount(Money amount)
    {
        var discountAmount = Math.Min(amount.Amount, FixedAmount.Amount);
        return Money.FromDecimal(discountAmount).Value;
    }
}

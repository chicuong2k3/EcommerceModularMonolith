namespace Promotion.Domain.CouponAggregate;

public abstract class Discount : Entity
{
    public abstract Money CalculateDiscountAmount(Money amount);
}

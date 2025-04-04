namespace Promotion.Domain.CouponAggregate;

public interface IDiscountService
{
    Task<Discount?> GetByTypeAndValueAsync(string discountType, decimal discountValue);
}

using Promotion.Domain.ConditionAggregate;

namespace Promotion.Domain.CouponAggregate;

/// <summary>
/// Represents a discount-based promotion (e.g., percentage off or fixed amount)
/// </summary>
public class Coupon : AggregateRoot
{
    public string Code { get; }
    public string? Description { get; }
    public Discount Discount { get; }
    public DateTime ExpiryDate { get; }
    public int UsageLimit { get; }
    public int CurrentUsageCount { get; private set; }
    public List<Condition> Conditions { get; private set; }

    private Coupon()
    {
    }

    private Coupon(
        string code,
        Discount discount,
        DateTime expiryDate,
        int usageLimit,
        string? description,
        List<Condition>? conditions = null)
    {
        Id = Guid.NewGuid();
        Code = code;
        Discount = discount;
        ExpiryDate = expiryDate;
        UsageLimit = usageLimit;
        CurrentUsageCount = 0;
        Description = description;
        Conditions = conditions ?? new List<Condition>();
    }

    public static async Task<Result<Coupon>> CreateAsync(
        IDiscountService discountService,
        string code,
        string discountType,
        decimal discountValue,
        DateTime expiryDate,
        int usageLimit,
        string? description,
        List<Condition>? conditions = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Fail(new ValidationError("Coupon code is required"));

        if (expiryDate < DateTime.UtcNow)
            return Result.Fail(new ValidationError("Expiry date must be in the future."));

        if (usageLimit <= 0)
            return Result.Fail(new ValidationError("Usage limit must be greater than 0."));

        var discountCreationResult = await DiscountFactory.CreateDiscountAsync(discountType, discountValue, discountService);

        if (discountCreationResult.IsFailed)
            return Result.Fail(discountCreationResult.Errors);

        return Result.Ok(new Coupon(code, discountCreationResult.Value, expiryDate, usageLimit, description, conditions));
    }

    public bool IsValid()
    {
        return ExpiryDate >= DateTime.UtcNow && CurrentUsageCount < UsageLimit;
    }

    public Result<Money> ApplyToOrder(OrderDetails orderDetails)
    {
        if (CurrentUsageCount >= UsageLimit)
            return Result.Fail("Coupon usage limit exceeded.");

        if (!IsValid())
            return Result.Fail("Coupon is invalid or expired.");

        if (Conditions.Any() && !Conditions.All(c => c.IsSatisfied(orderDetails)))
            return Result.Fail("Order does not meet coupon conditions.");


        CurrentUsageCount++;

        return Result.Ok(Discount.CalculateDiscountAmount(orderDetails.Subtotal));
    }
}

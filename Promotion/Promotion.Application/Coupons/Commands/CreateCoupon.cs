using Promotion.Application.Coupons.ReadModels;

namespace Promotion.Application.Coupons.Commands;

public record CreateCoupon(
    string CouponCode,
    string DiscountType,
    decimal DiscountValue,
    DateTime ExpiryDate,
    int UsageLimit,
    string? Description,
    List<Guid>? ConditionIds) : ICommand<CouponReadModel>;

internal sealed class CreateCouponHandler(
    ICouponRepository couponRepository,
    IConditionRepository conditionRepository,
    IDiscountService discountService)
    : ICommandHandler<CreateCoupon, CouponReadModel>
{
    public async Task<Result<CouponReadModel>> Handle(CreateCoupon command, CancellationToken cancellationToken)
    {
        var conditions = await conditionRepository.GetConditionsAsync(command.ConditionIds, cancellationToken);

        var couponCreationResult = await Coupon.CreateAsync(
            discountService,
            command.CouponCode,
            command.DiscountType,
            command.DiscountValue,
            command.ExpiryDate,
            command.UsageLimit,
            command.Description,
            conditions.ToList());

        if (couponCreationResult.IsFailed)
        {
            return Result.Fail(couponCreationResult.Errors);
        }

        var coupon = couponCreationResult.Value;

        await couponRepository.AddAsync(coupon, cancellationToken);


        string discountType = string.Empty;
        string discountValue = string.Empty;
        if (coupon.Discount is PercentageDiscount percentageDiscount)
        {
            discountType = "Percentage";
            discountValue = percentageDiscount.Percentage.ToString();

        }
        else if (coupon.Discount is FixedAmountDiscount fixedAmountDiscount)
        {
            discountType = "FixedAmount";
            discountValue = fixedAmountDiscount.FixedAmount.Amount.ToString();
        }
        return Result.Ok(new CouponReadModel()
        {
            Id = coupon.Id,
            Code = coupon.Code,
            DiscountType = discountType,
            DiscountValue = discountValue,
            ExpiryDate = coupon.ExpiryDate,
            UsageLimit = coupon.UsageLimit,
            CurrentUsageCount = coupon.CurrentUsageCount,
            Description = coupon.Description
        });
    }
}

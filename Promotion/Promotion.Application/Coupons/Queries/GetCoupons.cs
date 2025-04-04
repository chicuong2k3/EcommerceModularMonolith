using Promotion.Application.Conditions.ReadModels;
using Promotion.Application.Coupons.ReadModels;

namespace Promotion.Application.Coupons.Queries;

public record GetCoupons() : IQuery<List<CouponReadModel>>;

internal sealed class GetCouponsHandler(ICouponRepository couponRepository)
    : IQueryHandler<GetCoupons, List<CouponReadModel>>
{
    public async Task<Result<List<CouponReadModel>>> Handle(GetCoupons query, CancellationToken cancellationToken)
    {
        var coupons = await couponRepository.GetAllAsync(cancellationToken);
        var couponReadModels = coupons.Select(coupon => new CouponReadModel
        {
            Id = coupon.Id,
            Code = coupon.Code,
            DiscountType = coupon.Discount switch
            {
                PercentageDiscount percentageDiscount => "Percentage",
                FixedAmountDiscount fixedAmountDiscount => "FixedAmount",
                _ => "Unknown"
            },
            DiscountValue = coupon.Discount switch
            {
                PercentageDiscount percentageDiscount => percentageDiscount.Percentage.ToString(),
                FixedAmountDiscount fixedAmountDiscount => fixedAmountDiscount.FixedAmount.Amount.ToString(),
                _ => "Unknown"
            },
            ExpiryDate = coupon.ExpiryDate,
            UsageLimit = coupon.UsageLimit,
            CurrentUsageCount = coupon.CurrentUsageCount,
            Description = coupon.Description,
            Conditions = coupon.Conditions.Select(condition => new ConditionReadModel
            {
                Id = condition.Id,
                Name = condition.Name,
                ConditionType = condition.Type.ToString(),
                Value = condition.Value
            }).ToList()
        }).ToList();
        return Result.Ok(couponReadModels);
    }
}

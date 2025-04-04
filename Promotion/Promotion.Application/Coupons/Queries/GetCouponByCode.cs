using Promotion.Application.Conditions.ReadModels;
using Promotion.Application.Coupons.ReadModels;

namespace Promotion.Application.Coupons.Queries;

public record GetCouponByCode(string CouponCode) : IQuery<CouponReadModel>;

internal sealed class GetCouponByCodeHandler(ICouponRepository couponRepository)
    : IQueryHandler<GetCouponByCode, CouponReadModel>
{
    public async Task<Result<CouponReadModel>> Handle(GetCouponByCode query, CancellationToken cancellationToken)
    {
        var coupon = await couponRepository.GetByCodeAsync(query.CouponCode, cancellationToken);

        if (coupon == null)
        {
            return Result.Fail(new NotFoundError($"Coupon with code '{query.CouponCode}' not found"));
        }

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

        var couponReadModel = new CouponReadModel()
        {
            Id = coupon.Id,
            Code = coupon.Code,
            DiscountType = discountType,
            DiscountValue = discountValue,
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
        };

        return Result.Ok(couponReadModel);
    }
}

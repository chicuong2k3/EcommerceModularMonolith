namespace Promotion.Domain.CouponAggregate;

internal class DiscountFactory
{
    public static async Task<Result<Discount>> CreateDiscountAsync(
        string discountType,
        decimal value,
        IDiscountService discountService)
    {
        var validDiscountTypes = "Percentage, FixedAmount";

        var discount = await discountService.GetByTypeAndValueAsync(discountType, value);

        if (discount != null)
        {
            return Result.Ok(discount);
        }

        if (discountType == "Percentage")
        {
            var percentageDiscountResult = PercentageDiscount.Create((double)value);
            return percentageDiscountResult.IsSuccess
                ? Result.Ok<Discount>(percentageDiscountResult.Value)
                : Result.Fail(percentageDiscountResult.Errors);
        }
        else if (discountType == "FixedAmount")
        {
            var moneyCreationResult = Money.FromDecimal(value);
            if (moneyCreationResult.IsFailed)
            {
                return Result.Fail(moneyCreationResult.Errors);
            }
            var fixedAmountDiscount = new FixedAmountDiscount(moneyCreationResult.Value);

            return Result.Ok<Discount>(fixedAmountDiscount);
        }

        return Result.Fail(new ValidationError($"Invalid discount type. Valid discount types: {validDiscountTypes}"));
    }
}

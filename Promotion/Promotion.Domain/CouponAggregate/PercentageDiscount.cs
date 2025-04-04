using System.ComponentModel.DataAnnotations;

namespace Promotion.Domain.CouponAggregate;

public class PercentageDiscount : Discount
{
    public double Percentage { get; }

    private PercentageDiscount()
    {
    }

    private PercentageDiscount(double percentage)
    {
        Id = Guid.NewGuid();
        Percentage = percentage;
    }

    public static Result<PercentageDiscount> Create(double percentage)
    {
        if (percentage <= 0 || percentage > 100)
        {
            return Result.Fail(new ValidationError("Percentage must be between 0 and 100"));
        }

        return Result.Ok(new PercentageDiscount(percentage));
    }

    public override Money CalculateDiscountAmount(Money amount)
    {
        return amount * ((decimal)Percentage / 100);
    }
}

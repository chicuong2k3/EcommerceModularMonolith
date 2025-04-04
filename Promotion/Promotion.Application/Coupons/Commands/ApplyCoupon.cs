using Promotion.Domain.Common;
namespace Promotion.Application.Coupons.Commands;

public record ApplyCoupon(string CouponCode, Guid OrderId, Money Subtotal) : ICommand<Money>;

internal sealed class ApplyCouponHandler(ICouponRepository couponRepository)
    : ICommandHandler<ApplyCoupon, Money>
{
    public async Task<Result<Money>> Handle(ApplyCoupon command, CancellationToken cancellationToken)
    {
        var coupon = await couponRepository.GetByCodeAsync(command.CouponCode, cancellationToken);

        if (coupon == null)
            return Result.Fail(new ValidationError($"Coupon with code '{command.CouponCode}' not found"));

        var result = coupon.ApplyToOrder(new OrderDetails()
        {
            OrderId = command.OrderId,
            Subtotal = command.Subtotal
        });

        if (result.IsFailed)
            return Result.Fail<Money>(result.Errors);

        await couponRepository.SaveChangesAsync(cancellationToken);

        return result;
    }
}

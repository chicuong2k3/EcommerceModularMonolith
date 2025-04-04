namespace Promotion.Application.Coupons.Commands;

public record DeleteCoupon(string CouponCode) : ICommand;

internal sealed class DeleteCouponHandler(ICouponRepository couponRepository)
    : ICommandHandler<DeleteCoupon>
{
    public async Task<Result> Handle(DeleteCoupon command, CancellationToken cancellationToken)
    {
        var coupon = await couponRepository.GetByCodeAsync(command.CouponCode, cancellationToken);
        if (coupon == null)
            return Result.Fail(new ValidationError($"Coupon with code '{command.CouponCode}' not found"));

        await couponRepository.RemoveAsync(coupon, cancellationToken);
        return Result.Ok();
    }
}

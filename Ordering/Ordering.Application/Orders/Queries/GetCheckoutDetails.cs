using Ordering.Application.Orders.ReadModels;

namespace Ordering.Application.Orders.Queries;

public record GetCheckoutDetails(Guid CustomerId, string CouponCode) : ICommand<CheckoutDetailsReadModel>;

internal class GetCheckoutDetailsHandler(
    ICartRepository cartRepository,
    ICouponService couponService)
    : ICommandHandler<GetCheckoutDetails, CheckoutDetailsReadModel>
{
    public async Task<Result<CheckoutDetailsReadModel>> Handle(GetCheckoutDetails query, CancellationToken cancellationToken)
    {
        //var shippingFee = await shippingService.CalculateShippingFee(request.ShippingAddress, request.ShippingMethod);
        var shippingFee = 0;

        var cart = await cartRepository.GetAsync(query.CustomerId, cancellationToken);

        if (cart == null)
            return Result.Fail(new NotFoundError("Cart not found"));

        var cartItems = cart.Items.ToList();

        if (!cartItems.Any())
            return Result.Fail("There is no items in cart");

        //var discount = string.IsNullOrEmpty(query.CouponCode)
        //    ? 0
        //    : await couponService.ApplyCouponAsync(query.CouponCode, query.CartItems);
        //var subTotal = cartItems.Sum(item => item.SalePrice * item.Quantity);
        //var totalAmount = subTotal + shippingFee - discount;

        return Result.Ok(new CheckoutDetailsReadModel
        {

        });
    }

}
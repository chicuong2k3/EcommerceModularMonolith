using FluentResults;
using Ordering.Core.ReadModels;
using Ordering.Core.Repositories;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;

namespace Ordering.Core.Queries;

public record GetCheckoutDetails(Guid CustomerId, string CouponCode) : IQuery<CheckoutDetailsReadModel>;

internal class GetCheckoutDetailsHandler(
    ICartRepository cartRepository)
    : IQueryHandler<GetCheckoutDetails, CheckoutDetailsReadModel>
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
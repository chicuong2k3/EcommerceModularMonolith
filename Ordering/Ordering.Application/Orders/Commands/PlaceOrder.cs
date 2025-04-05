namespace Ordering.Application.Orders.Commands;

public record PlaceOrder(
    Guid CustomerId,
    string? Street,
    string Ward,
    string District,
    string Province,
    string Country,
    string PaymentMethod,
    string ShippingMethod,
    string? CouponCode) : ICommand<Order>;

//internal sealed class PlaceOrderHandler(
//    IOrderRepository orderRepository,
//    ICartRepository cartRepository,
//    ICouponService couponService)
//    : ICommandHandler<PlaceOrder, Order>
//{
//    public async Task<Result<Order>> Handle(PlaceOrder command, CancellationToken cancellationToken)
//    {
//        if (!Enum.TryParse<PaymentMethod>(command.PaymentMethod, out var paymentMethod))
//            return Result.Fail(new ValidationError("Invalid payment method"));

//        var cart = await cartRepository.GetAsync(command.CustomerId, cancellationToken);

//        if (cart == null)
//            return Result.Fail(new NotFoundError("Cart not found"));

//        var cartItems = cart.Items.ToList();

//        if (!cartItems.Any())
//            return Result.Fail("There is no items in cart");

//        var orderItems = new List<OrderItem>();

//        var orderItemCreationResults = new List<Result<OrderItem>>();
//        foreach (var item in cartItems)
//        {
//            // check item availability

//            var orderItemCreationResult = OrderItem.Create(
//                item.ProductId,
//                item.ProductVariantId,
//                item.Quantity);

//            orderItemCreationResults.Add(orderItemCreationResult);
//        }

//        if (orderItemCreationResults.Any(r => r.IsFailed))
//        {
//            var errors = orderItemCreationResults
//                .Where(r => r.IsFailed)
//                .SelectMany(r => r.Errors)
//                .ToList();
//            return Result.Fail(errors);
//        }

//        orderItems.AddRange(orderItemCreationResults.Select(r => r.Value));

//        var locationCreationResult = Location.Create(
//                command.Street,
//                command.Ward,
//                command.District,
//                command.Province,
//                command.Country);


//        var paymentInfoCreationResult = PaymentInfo.Create(command.PaymentMethod);
//        if (paymentInfoCreationResult.IsFailed)
//            return Result.Fail(paymentInfoCreationResult.Errors);

//        var shippingCosts = Money.FromDecimal(0).Value;
//        if (locationCreationResult.IsFailed)
//            return Result.Fail(locationCreationResult.Errors);
//        var shippingInfo = new ShippingInfo(shippingCosts, locationCreationResult.Value);

//        var orderCreationResult = Order.Create(
//            command.CustomerId,
//            paymentInfoCreationResult.Value,
//            shippingInfo,
//            orderItems);

//        if (orderCreationResult.IsFailed)
//            return orderCreationResult;

//        var order = orderCreationResult.Value;

//        Money? discountAmount = null;
//        if (!string.IsNullOrWhiteSpace(command.CouponCode))
//        {
//            var orderDetails = new OrderDetails(
//                order.Subtotal
//            );
//            discountAmount = await couponService.ApplyCouponAsync(orderDetails, cancellationToken);
//        }

//        var placeOrderResult = order.Place(discountAmount);
//        if (placeOrderResult.IsFailed)
//            return placeOrderResult;

//        await orderRepository.AddAsync(order, cancellationToken);

//        return Result.Ok(order);
//    }
//}

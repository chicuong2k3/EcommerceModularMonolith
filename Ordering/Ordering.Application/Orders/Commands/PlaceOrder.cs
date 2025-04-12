using Ordering.Domain.OrderAggregate.Errors;

namespace Ordering.Application.Orders.Commands;

public record PlaceOrder(
    Guid CustomerId,
    string? Street,
    string Ward,
    string District,
    string Province,
    string Country,
    string PhoneNumber,
    string PaymentMethod,
    string ShippingMethod) : ICommand<Order>;

internal sealed class PlaceOrderHandler(
    IOrderRepository orderRepository,
    ICartRepository cartRepository,
    IProductRepository productRepository)
    : ICommandHandler<PlaceOrder, Order>
{
    public async Task<Result<Order>> Handle(PlaceOrder command, CancellationToken cancellationToken)
    {
        // Get cart and validate items
        var cart = await cartRepository.GetAsync(command.CustomerId, cancellationToken);

        if (cart == null)
            return Result.Fail(new CartEmptyError("There is no items in cart"));

        var cartItems = cart.Items.ToList();

        if (!cartItems.Any())
            return Result.Fail(new CartEmptyError("There is no items in cart"));

        // Create order items
        var orderItems = new List<OrderItem>();
        foreach (var item in cartItems)
        {
            var product = await productRepository.GetProductAsync(item.ProductId, item.ProductVariantId, cancellationToken);

            if (product == null)
                return Result.Fail(new NotFoundError($"Product with id '{item.ProductId}' not found"));

            // Check if the product is in stock
            if (product.Quantity < item.Quantity)
                return Result.Fail(new OutOfStockError(product.Name, product.Quantity, item.Quantity));

            var originalPrice = Money.FromDecimal(product.OriginalPrice).Value;
            var salePrice = product.SalePrice != null
                ? Money.FromDecimal(product.SalePrice.Value).Value : originalPrice;

            var orderItemCreationResult = OrderItem.Create(
                item.ProductId,
                item.ProductVariantId,
                product.Name,
                item.Quantity,
                originalPrice,
                salePrice,
                product.ImageUrl,
                product.AttributesDescription);

            if (orderItemCreationResult.IsFailed)
                return Result.Fail(orderItemCreationResult.Errors);

            orderItems.Add(orderItemCreationResult.Value);
        }

        // Initialize order
        var locationCreationResult = Location.Create(
                command.Street,
                command.Ward,
                command.District,
                command.Province,
                command.Country);
        if (locationCreationResult.IsFailed)
            return Result.Fail(locationCreationResult.Errors);

        // Create shipping info
        var shippingInfoCreationResult = ShippingInfo.Create(Money.FromDecimal(0).Value, locationCreationResult.Value, command.PhoneNumber);
        if (shippingInfoCreationResult.IsFailed)
            return Result.Fail(shippingInfoCreationResult.Errors);

        // Create payment info
        var paymentInfoCreationResult = PaymentInfo.Create(command.PaymentMethod);
        if (paymentInfoCreationResult.IsFailed)
            return Result.Fail(paymentInfoCreationResult.Errors);


        var orderCreationResult = Order.Create(
            command.CustomerId,
            paymentInfoCreationResult.Value,
            shippingInfoCreationResult.Value,
            orderItems);

        if (orderCreationResult.IsFailed)
            return orderCreationResult;

        var order = orderCreationResult.Value;

        // notify payment service
        order.RaisePaymentEvent();

        await orderRepository.AddAsync(order, cancellationToken);

        return Result.Ok(order);
    }
}

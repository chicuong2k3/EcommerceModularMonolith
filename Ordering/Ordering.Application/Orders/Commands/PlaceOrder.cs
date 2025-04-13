using Catalog.Contracts;
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
    IProductService productService)
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
            var product = await productService.GetProductByIdAsync(item.ProductId, cancellationToken);

            if (product == null)
                return Result.Fail(new NotFoundError($"Product with id '{item.ProductId}' not found"));

            var productVariant = product.Variants.FirstOrDefault(v => v.VariantId == item.ProductVariantId);
            if (productVariant == null)
                return Result.Fail(new NotFoundError($"Product variant with id '{item.ProductVariantId}' not found"));

            // Check if the product is in stock
            if (productVariant.Quantity < item.Quantity)
                return Result.Fail(new OutOfStockError(product.Name, productVariant.Quantity, item.Quantity));

            var originalPrice = Money.FromDecimal(productVariant.OriginalPrice).Value;
            var salePrice = productVariant.SalePrice != null
                ? Money.FromDecimal(productVariant.SalePrice.Value).Value : originalPrice;

            var orderItemCreationResult = OrderItem.Create(
                item.ProductId,
                item.ProductVariantId,
                product.Name,
                item.Quantity,
                originalPrice,
                salePrice,
                productVariant.ImageUrl,
                string.Join(", ", productVariant.Attributes.Select(a => $"{a.Key}: {a.Value}")));

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

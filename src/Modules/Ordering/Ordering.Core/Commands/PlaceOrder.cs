using Catalog.Contracts;
using FluentResults;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;
using Ordering.Core.ValueObjects;
using Shared.Abstractions.Application;
using Shared.Abstractions.Core;

namespace Ordering.Core.Commands;

public record PlaceOrder(
    Guid CustomerId,
    string? Street,
    string Ward,
    string District,
    string Province,
    string Country,
    string PhoneNumber,
    string PaymentMethod,
    string ShippingMethod) : ICommand<Guid>;

internal sealed class PlaceOrderHandler(
    IOrderRepository orderRepository,
    ICartRepository cartRepository,
    IProductService productService)
    : ICommandHandler<PlaceOrder, Guid>
{
    public async Task<Result<Guid>> Handle(PlaceOrder command, CancellationToken cancellationToken)
    {
        // Get cart and validate items
        var cart = await cartRepository.GetAsync(command.CustomerId, cancellationToken);

        if (cart == null)
            return Result.Fail(new Error("There is no items in cart"));

        var cartItems = cart.Items.ToList();

        if (!cartItems.Any())
            return Result.Fail(new Error("There is no items in cart"));

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
                return Result.Fail(new Error(""));

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
        if (!Enum.TryParse<ShippingMethod>(command.ShippingMethod, out var shippingMethod))
            return Result.Fail($"Invalid shipping method: {command.ShippingMethod}");

        var shippingInfoCreationResult = ShippingInfo.Create(locationCreationResult.Value, command.PhoneNumber, shippingMethod);
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
            return Result.Fail(orderCreationResult.Errors);

        var order = orderCreationResult.Value;

        // notify payment service
        order.RaisePaymentEvent();

        await orderRepository.AddAsync(order, cancellationToken);

        return Result.Ok(order.Id);
    }
}

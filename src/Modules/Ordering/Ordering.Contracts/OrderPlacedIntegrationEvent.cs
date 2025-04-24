using Shared.Abstractions.Application;

namespace Ordering.Contracts;

public class OrderPlacedIntegrationEvent : IntegrationEvent
{
    public OrderPlacedIntegrationEvent(
        Guid orderId,
        Guid customerId,
        List<OrderItemMessage> orderItems)
    {
        OrderId = orderId;
        CustomerId = customerId;
        OrderItems = orderItems;
    }

    public Guid OrderId { get; }
    public Guid CustomerId { get; }
    public List<OrderItemMessage> OrderItems { get; }
}

public class OrderItemMessage
{
    public OrderItemMessage(Guid productId,
                            Guid productVariantId,
                            int quantity)
    {
        ProductId = productId;
        ProductVariantId = productVariantId;
        Quantity = quantity;
    }

    public Guid ProductId { get; }
    public Guid ProductVariantId { get; }
    public int Quantity { get; }
}
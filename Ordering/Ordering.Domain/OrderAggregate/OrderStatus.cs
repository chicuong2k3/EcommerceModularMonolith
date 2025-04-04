namespace Ordering.Domain.OrderAggregate;

public enum OrderStatus
{
    Placed,         // Confirmed
    Paid,           // Payment completed
    Processing,     // Inventory allocated, preparing for shipment
    Shipped,        // Handed to logistics
    Delivered,      // Successfully delivered
    Canceled,       // Abandoned before fulfillment
    Refunded        // Refund issued post-delivery
}
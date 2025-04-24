namespace Ordering.Core.ValueObjects;

public enum OrderStatus
{
    PendingPayment, // Awaiting payment, the initial state when the order is created
    Paid,           // Payment completed
    Processing,     // Preparing for shipment
    Shipped,        // Handed to logistics
    Delivered,      // Successfully delivered
    Canceled,       // Abandoned before fulfillment
    Refunded        // Refund issued post-delivery
}

// COD: PendingPayment -> Processing
// Online: PendingPayment -> Paid -> Processing
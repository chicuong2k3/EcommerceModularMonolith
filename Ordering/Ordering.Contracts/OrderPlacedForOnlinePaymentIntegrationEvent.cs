using Common.Messages;

namespace Ordering.Contracts;

public class OrderPlacedForOnlinePaymentIntegrationEvent : IntegrationEvent
{
    public OrderPlacedForOnlinePaymentIntegrationEvent(
        Guid orderId,
        Guid customerId,
        decimal totalAmount,
        string paymentMethod)
    {
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        PaymentMethod = paymentMethod;
    }

    public Guid OrderId { get; }
    public Guid CustomerId { get; }
    public decimal TotalAmount { get; }
    public string PaymentMethod { get; }
}

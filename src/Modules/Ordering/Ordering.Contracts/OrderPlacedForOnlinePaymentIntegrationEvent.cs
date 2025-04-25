
using Shared.Abstractions.Application;

namespace Ordering.Contracts;

public class OrderPlacedForOnlinePaymentIntegrationEvent : IntegrationEvent
{
    public OrderPlacedForOnlinePaymentIntegrationEvent(
        Guid orderId,
        Guid customerId,
        decimal totalAmount,
        string paymentProvider)
    {
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        PaymentProvider = paymentProvider;
    }

    public Guid OrderId { get; }
    public Guid CustomerId { get; }
    public decimal TotalAmount { get; }
    public string PaymentProvider { get; }
}

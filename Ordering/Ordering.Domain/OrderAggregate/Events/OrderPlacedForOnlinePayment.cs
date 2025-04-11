namespace Ordering.Domain.OrderAggregate.Events;

public record OrderPlacedForOnlinePayment(
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount,
    string PaymentMethod) : DomainEvent;

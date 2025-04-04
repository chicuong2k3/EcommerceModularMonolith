namespace Ordering.Domain.OrderAggregate.Events;

public record OrderPlaced(
    Guid OrderId,
    Guid CustomerId,
    IEnumerable<OrderItem> OrderItems) : DomainEvent;

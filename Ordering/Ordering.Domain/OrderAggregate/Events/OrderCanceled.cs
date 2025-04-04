namespace Ordering.Domain.OrderAggregate.Events;

public record OrderCanceled(Guid OrderId) : DomainEvent;

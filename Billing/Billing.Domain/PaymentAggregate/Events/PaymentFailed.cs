namespace Billing.Domain.PaymentAggregate.Events;

public record PaymentFailed(Guid OrderId, IEnumerable<string> Messages) : DomainEvent;
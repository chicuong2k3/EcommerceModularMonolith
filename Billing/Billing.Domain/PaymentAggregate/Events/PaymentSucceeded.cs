namespace Billing.Domain.PaymentAggregate.Events;

public record PaymentSucceeded(Guid OrderId) : DomainEvent;
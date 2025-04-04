namespace Payment.Domain.PaymentAggregate.Events;

public record PaymentSucceeded(Guid PaymentId) : DomainEvent;
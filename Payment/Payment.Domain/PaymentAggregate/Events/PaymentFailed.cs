namespace Payment.Domain.PaymentAggregate.Events;

public record PaymentFailed(Guid PaymentId, string Reason) : DomainEvent;
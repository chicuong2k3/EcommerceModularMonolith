using Shared.Abstractions.Core;

namespace Pay.Core.Events;

public record PaymentRefunded(Guid PaymentId, decimal Amount, string Reason) : DomainEvent;

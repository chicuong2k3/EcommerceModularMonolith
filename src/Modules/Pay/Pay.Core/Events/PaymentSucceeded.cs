using Shared.Abstractions.Core;

namespace Pay.Core.Events;

public record PaymentSucceeded(Guid OrderId) : DomainEvent;
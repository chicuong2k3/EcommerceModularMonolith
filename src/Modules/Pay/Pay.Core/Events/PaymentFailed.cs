using Shared.Abstractions.Core;

namespace Pay.Core.Events;

public record PaymentFailed(Guid OrderId, IEnumerable<string> Messages) : DomainEvent;
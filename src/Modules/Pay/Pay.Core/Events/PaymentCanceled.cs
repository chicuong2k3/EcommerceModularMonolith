using Shared.Abstractions.Core;

namespace Pay.Core.Events;

public record PaymentCanceled(Guid OrderId) : DomainEvent;

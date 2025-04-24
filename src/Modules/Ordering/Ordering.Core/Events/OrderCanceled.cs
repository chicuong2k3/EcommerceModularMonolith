using Shared.Abstractions.Core;

namespace Ordering.Core.Events;

public record OrderCanceled(Guid OrderId) : DomainEvent;

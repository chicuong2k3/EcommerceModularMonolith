using Shared.Abstractions.Core;

namespace Ordering.Core.Events;

public record OrderPlacedForOnlinePayment(
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount,
    string PaymentMethod) : DomainEvent;

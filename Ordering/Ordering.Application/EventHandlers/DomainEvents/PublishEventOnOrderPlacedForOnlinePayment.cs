using Ordering.Contracts;
using Ordering.Domain.OrderAggregate.Events;

namespace Ordering.Application.EventHandlers.DomainEvents;

internal class PublishEventOnOrderPlacedForOnlinePayment
    : DomainEventHandler<OrderPlacedForOnlinePayment>
{
    private readonly ILogger<PublishEventOnOrderPlacedForOnlinePayment> logger;
    private readonly IEventBus eventBus;

    public PublishEventOnOrderPlacedForOnlinePayment(
        ILogger<PublishEventOnOrderPlacedForOnlinePayment> logger,
        IEventBus eventBus)
    {
        this.logger = logger;
        this.eventBus = eventBus;
    }

    public override async Task Handle(OrderPlacedForOnlinePayment domainEvent, CancellationToken cancellationToken = default)
    {
        var @event = new OrderPlacedForOnlinePaymentIntegrationEvent
        (
            domainEvent.OrderId,
            domainEvent.CustomerId,
            domainEvent.TotalAmount,
            domainEvent.PaymentMethod
        );

        await eventBus.PublishAsync(@event, cancellationToken);

        logger.LogInformation("Published OrderPlacedForOnlinePaymentIntegrationEvent to message broker");
    }
}

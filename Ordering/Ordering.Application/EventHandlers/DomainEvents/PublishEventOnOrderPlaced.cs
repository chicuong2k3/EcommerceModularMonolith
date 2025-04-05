using Ordering.Contracts;
using Ordering.Domain.OrderAggregate.Events;

namespace Ordering.Application.EventHandlers.DomainEvents;

internal class PublishEventOnOrderPlaced : DomainEventHandler<OrderPlaced>
{
    private readonly ILogger<PublishEventOnOrderPlaced> logger;
    private readonly IEventBus eventBus;

    public PublishEventOnOrderPlaced(
        ILogger<PublishEventOnOrderPlaced> logger,
        IEventBus eventBus)
    {
        this.logger = logger;
        this.eventBus = eventBus;
    }

    public override async Task Handle(OrderPlaced domainEvent, CancellationToken cancellationToken)
    {
        var orderPlacedEvent = new OrderPlacedIntegrationEvent
        (
            domainEvent.OrderId,
            domainEvent.CustomerId,
            domainEvent.OrderItems.Select(orderItem => new OrderItemMessage
            (
                orderItem.ProductId,
                orderItem.ProductVariantId,
                orderItem.Quantity
            )).ToList()
        );

        await eventBus.PublishAsync(orderPlacedEvent, cancellationToken);

        logger.LogInformation("Published order placed event to message broker");
    }
}

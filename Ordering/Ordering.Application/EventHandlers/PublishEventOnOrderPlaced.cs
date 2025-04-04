using Ordering.Contracts;
using Ordering.Domain.OrderAggregate.Events;

namespace Ordering.Application.EventHandlers;

internal class PublishEventOnOrderPlaced : IDomainEventHandler<OrderPlaced>
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

    public async Task Handle(OrderPlaced notification, CancellationToken cancellationToken)
    {
        var orderPlacedEvent = new OrderPlacedIntegrationEvent
        (
            notification.OrderId,
            notification.CustomerId,
            notification.OrderItems.Select(orderItem => new OrderItemMessage
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

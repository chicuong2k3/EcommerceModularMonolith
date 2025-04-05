

using Catalog.Contracts;
using Catalog.Domain.ProductAggregate.Events;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.EventHandlers.DomainEvents;

internal class PublishEventOnProductVariantAdded
    : DomainEventHandler<ProductVariantAdded>
{
    private readonly IEventBus eventBus;
    private readonly ILogger<PublishEventOnProductVariantAdded> logger;

    public PublishEventOnProductVariantAdded(
        IEventBus eventBus,
        ILogger<PublishEventOnProductVariantAdded> logger)
    {
        this.eventBus = eventBus;
        this.logger = logger;
    }

    public override async Task Handle(ProductVariantAdded domainEvent, CancellationToken cancellationToken)
    {
        var @event = new ProductVariantAddedIntegrationEvent()
        {
            ProductId = domainEvent.ProductId,
            VariantId = domainEvent.ProductVariantId,
            OriginalPrice = domainEvent.OriginalPrice,
            SalePrice = domainEvent.SalePrice,
            Quantity = domainEvent.Quantity,
            ImageUrl = domainEvent.ImageUrl,
            Attributes = domainEvent.Attributes
        };
        logger.LogInformation("Publishing ProductVariantAddedIntegrationEvent: {@event}", @event);
        await eventBus.PublishAsync(@event, cancellationToken);
        logger.LogInformation("Published ProductVariantAddedIntegrationEvent: {@event}", @event);
    }
}

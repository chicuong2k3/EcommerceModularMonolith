

using Catalog.Contracts;
using Catalog.Domain.ProductAggregate.Events;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.EventHandlers.DomainEvents;

internal class PublishEventOnVariantAdded : DomainEventHandler<ProductVariantAdded>
{
    private readonly IEventBus eventBus;
    private readonly ILogger<PublishEventOnVariantAdded> logger;

    public PublishEventOnVariantAdded(IEventBus eventBus, ILogger<PublishEventOnVariantAdded> logger)
    {
        this.eventBus = eventBus;
        this.logger = logger;
    }

    public override async Task Handle(ProductVariantAdded domainEvent, CancellationToken cancellationToken = default)
    {
        var integrationEvent = new VariantAddedIntegrationEvent(
            domainEvent.ProductId,
            domainEvent.ProductVariantId,
            domainEvent.ProductName,
            domainEvent.OriginalPrice,
            domainEvent.Quantity,
            domainEvent.ImageUrl,
            domainEvent.SalePrice,
            domainEvent.Attributes);

        await eventBus.PublishAsync(integrationEvent, cancellationToken);
        logger.LogInformation("Published integration event: {IntegrationEvent}", integrationEvent);
    }
}

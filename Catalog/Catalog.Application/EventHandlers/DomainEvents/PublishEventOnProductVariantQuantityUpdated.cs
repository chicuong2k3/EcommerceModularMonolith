using Catalog.Domain.ProductAggregate.Events;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.EventHandlers.DomainEvents;

internal class PublishEventOnProductVariantQuantityUpdated
    : DomainEventHandler<ProductVariantQuantityUpdated>
{
    private readonly ILogger<PublishEventOnProductVariantQuantityUpdated> logger;

    public PublishEventOnProductVariantQuantityUpdated(ILogger<PublishEventOnProductVariantQuantityUpdated> logger)
    {
        this.logger = logger;
    }
    public override async Task Handle(ProductVariantQuantityUpdated domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Product variant quantity updated. ProductId: {ProductId}, ProductVariantId: {ProductVariantId}, NewQuantity: {NewQuantity}", domainEvent.ProductId, domainEvent.ProductVariantId, domainEvent.NewQuantity);
    }
}

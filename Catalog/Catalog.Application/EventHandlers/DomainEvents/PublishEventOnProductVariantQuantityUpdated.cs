using Catalog.Domain.ProductAggregate.Events;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.EventHandlers.DomainEvents;

internal class PublishEventOnProductVariantQuantityUpdated
    : IDomainEventHandler<ProductVariantQuantityUpdated>
{
    private readonly ILogger<PublishEventOnProductVariantQuantityUpdated> logger;

    public PublishEventOnProductVariantQuantityUpdated(ILogger<PublishEventOnProductVariantQuantityUpdated> logger)
    {
        this.logger = logger;
    }
    public async Task Handle(ProductVariantQuantityUpdated notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Product variant quantity updated. ProductId: {ProductId}, ProductVariantId: {ProductVariantId}, NewQuantity: {NewQuantity}", notification.ProductId, notification.ProductVariantId, notification.NewQuantity);
    }
}

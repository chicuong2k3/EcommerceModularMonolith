using Catalog.Core.Repositories;
using Microsoft.Extensions.Logging;
using Ordering.Contracts;
using Shared.Abstractions.Application;

namespace Catalog.Core.EventHandlers.IntegrationEvents;

public class UpdateProductQuantityOnOrderPlaced
    : IntegrationEventHandler<OrderPlacedIntegrationEvent>
{
    private readonly ILogger<UpdateProductQuantityOnOrderPlaced> logger;
    private readonly IProductRepository productRepository;

    public UpdateProductQuantityOnOrderPlaced(ILogger<UpdateProductQuantityOnOrderPlaced> logger,
                                              IProductRepository productRepository)
    {
        this.logger = logger;
        this.productRepository = productRepository;
    }

    public override async Task Handle(OrderPlacedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        foreach (var item in integrationEvent.OrderItems)
        {
            var product = await productRepository.GetByIdWithVariantsAsync(item.ProductId, cancellationToken);
            if (product == null)
            {
                logger.LogError("Product not found: {ProductId}", item.ProductId);
                throw new Exception($"Product not found: {item.ProductId}");
            }
            var result = product.UpdateQuantity(item.ProductVariantId, item.Quantity);
            if (result.IsFailed)
            {
                logger.LogError("Failed to update product quantity on order placed event: {Errors}", result.Errors);
                throw new Exception("Product quantity update failed");
            }
        }

        logger.LogInformation("Updated product quantity on order placed event");
    }
}

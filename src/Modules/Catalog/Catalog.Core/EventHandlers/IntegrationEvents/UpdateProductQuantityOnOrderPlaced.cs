using Catalog.Core.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Contracts;
using Shared.Abstractions.Application;

namespace Catalog.Core.EventHandlers.IntegrationEvents;

public class UpdateProductQuantityOnOrderPlaced
    : IntegrationEventHandler<OrderPlacedIntegrationEvent>
{
    private readonly ILogger<UpdateProductQuantityOnOrderPlaced> logger;
    private readonly IMediator mediator;

    public UpdateProductQuantityOnOrderPlaced(ILogger<UpdateProductQuantityOnOrderPlaced> logger,
                                              IMediator mediator)
    {
        this.logger = logger;
        this.mediator = mediator;
    }

    public override async Task Handle(OrderPlacedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        foreach (var item in integrationEvent.OrderItems)
        {
            var result = await mediator.Send(new UpdateQuantity(
                item.ProductId,
                item.ProductVariantId,
                item.Quantity));

            if (result.IsFailed)
            {
                logger.LogError("Failed to update product quantity on order placed event: {Errors}", result.Errors);
                throw new Exception("Product quantity update failed");
            }
        }

        logger.LogInformation("Updated product quantity on order placed event");
    }
}

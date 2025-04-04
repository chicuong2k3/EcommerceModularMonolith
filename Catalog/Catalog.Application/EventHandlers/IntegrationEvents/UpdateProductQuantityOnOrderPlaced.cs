using Catalog.Application.Products.Commands;
using Microsoft.Extensions.Logging;
using Ordering.Contracts;

namespace Catalog.Application.EventHandlers.IntegrationEvents;

public class UpdateProductQuantityOnOrderPlaced
    : IConsumer<OrderPlacedIntegrationEvent>
{
    private readonly ILogger<UpdateProductQuantityOnOrderPlaced> logger;
    private readonly IMediator mediator;

    public UpdateProductQuantityOnOrderPlaced(ILogger<UpdateProductQuantityOnOrderPlaced> logger,
                                              IMediator mediator)
    {
        this.logger = logger;
        this.mediator = mediator;
    }

    public async Task Consume(ConsumeContext<OrderPlacedIntegrationEvent> context)
    {
        foreach (var item in context.Message.OrderItems)
        {
            var result = await mediator.Send(new UpdateQuantity(
                item.ProductId,
                item.ProductVariantId,
                item.Quantity));

            if (result.IsFailed)
            {
                logger.LogError("Failed to update product quantity on order placed event: {Errors}", result.Errors);
                return;
            }
        }

        logger.LogInformation("Updated product quantity on order placed event");
    }
}

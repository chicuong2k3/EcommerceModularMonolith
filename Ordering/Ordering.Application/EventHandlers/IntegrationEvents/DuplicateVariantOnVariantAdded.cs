using Catalog.Contracts;
using MediatR;
using Ordering.Application.Products.Commands;

namespace Ordering.Application.EventHandlers.IntegrationEvents;

public class DuplicateVariantOnVariantAdded
    : IntegrationEventHandler<ProductVariantAddedIntegrationEvent>
{
    private readonly IMediator mediator;
    private readonly ILogger<DuplicateVariantOnVariantAdded> logger;

    public DuplicateVariantOnVariantAdded(
        IMediator mediator,
        ILogger<DuplicateVariantOnVariantAdded> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    public override async Task Handle(ProductVariantAddedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        var command = new AddVariantsToProduct(
                integrationEvent.ProductId,
                integrationEvent.VariantId,
                integrationEvent.OriginalPrice,
                integrationEvent.SalePrice,
                integrationEvent.Quantity,
                integrationEvent.ImageUrl,
                integrationEvent.Attributes);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            logger.LogError("Failed to duplicate variant: {VariantId}", integrationEvent.VariantId);
            return;
        }

        logger.LogInformation("Duplicated variant: {VariantId}", integrationEvent.VariantId);

    }
}

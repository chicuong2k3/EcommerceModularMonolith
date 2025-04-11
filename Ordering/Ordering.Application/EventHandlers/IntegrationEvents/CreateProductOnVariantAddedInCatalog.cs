using Catalog.Contracts;
using MediatR;
using Ordering.Application.Products.Commands;

namespace Ordering.Application.EventHandlers.IntegrationEvents;

internal class CreateProductOnVariantAddedInCatalog(
    IMediator mediator,
    ILogger<CreateProductOnVariantAddedInCatalog> logger)
    : IntegrationEventHandler<VariantAddedIntegrationEvent>
{
    public override async Task Handle(VariantAddedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        var command = new CreateProduct(
            integrationEvent.ProductId,
            integrationEvent.VariantId,
            integrationEvent.Name,
            integrationEvent.OriginalPrice,
            integrationEvent.SalePrice,
            integrationEvent.Quantity,
            integrationEvent.ImageUrl,
            integrationEvent.Attributes);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            logger.LogError("Failed to create product: {Error}", result.Errors);
            throw new Exception("Product creation failed");
        }
        else
        {
            logger.LogInformation("Product created successfully: {ProductId}", integrationEvent.ProductId);
        }
    }
}

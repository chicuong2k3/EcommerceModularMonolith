using Catalog.Contracts;
using MediatR;
using Ordering.Application.Products.Commands;

namespace Ordering.Application.EventHandlers.IntegrationEvents;

public class DuplicateProductOnProductCreated
    : IntegrationEventHandler<ProductCreatedIntegrationEvent>
{
    private readonly IMediator mediator;
    private readonly ILogger<DuplicateProductOnProductCreated> logger;

    public DuplicateProductOnProductCreated(
        IMediator mediator,
        ILogger<DuplicateProductOnProductCreated> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }


    public override async Task Handle(ProductCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        var command = new CreateProduct(integrationEvent.ProductId, integrationEvent.Name);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            logger.LogError("Failed to duplicate product: {ProductName}", integrationEvent.Name);
            return;
        }

        logger.LogInformation("Duplicated product: {ProductName}", integrationEvent.Name);
    }
}

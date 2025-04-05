using Catalog.Contracts;
using Catalog.Domain.ProductAggregate.Events;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.EventHandlers.DomainEvents;

internal class PublishEventOnProductCreated
    : DomainEventHandler<ProductCreated>
{
    private readonly IEventBus eventBus;
    private readonly ILogger<PublishEventOnProductCreated> logger;

    public PublishEventOnProductCreated(
        IEventBus eventBus,
        ILogger<PublishEventOnProductCreated> logger)
    {
        this.eventBus = eventBus;
        this.logger = logger;
    }

    public override async Task Handle(ProductCreated domainEvent, CancellationToken cancellationToken)
    {
        var @event = new ProductCreatedIntegrationEvent()
        {
            ProductId = domainEvent.ProductId,
            Name = domainEvent.Name
        };

        logger.LogInformation("Publishing ProductCreatedIntegrationEvent: {@event}", @event);
        await eventBus.PublishAsync(@event, cancellationToken);
        logger.LogInformation("Published ProductCreatedIntegrationEvent: {@event}", @event);
    }
}

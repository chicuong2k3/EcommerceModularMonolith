using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Abstractions.Application;

namespace Shared.Infrastructure.Inbox;

internal class IdempotentIntegratonEventHandler<TEvent, TDbContext> : IntegrationEventHandler<TEvent>
    where TEvent : IntegrationEvent
    where TDbContext : DbContext
{
    private readonly TDbContext dbContext;
    private readonly IIntegrationEventHandler<TEvent> innerHandler;
    private readonly ILogger<IdempotentIntegratonEventHandler<TEvent, TDbContext>> logger;

    public IdempotentIntegratonEventHandler(
        TDbContext dbContext,
        IIntegrationEventHandler<TEvent> innerHandler,
        ILogger<IdempotentIntegratonEventHandler<TEvent, TDbContext>> logger)
    {
        this.dbContext = dbContext;
        this.innerHandler = innerHandler;
        this.logger = logger;
    }

    public override async Task Handle(TEvent integrationEvent, CancellationToken cancellationToken)
    {
        var consumer = new InboxMessageConsumer(integrationEvent.Id, innerHandler.GetType().Name);

        var existingConsumer = await dbContext.Set<InboxMessageConsumer>()
            .FirstOrDefaultAsync(x => x.InboxMessageId == consumer.InboxMessageId && x.Name == consumer.Name,
                                 cancellationToken);

        if (existingConsumer != null)
        {
            logger.LogInformation($"Event {integrationEvent.Id} already processed.");
            return;
        }


        await innerHandler.Handle(integrationEvent, cancellationToken);

        dbContext.Set<InboxMessageConsumer>().Add(consumer);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

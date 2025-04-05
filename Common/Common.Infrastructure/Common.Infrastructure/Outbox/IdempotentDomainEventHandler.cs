using Common.Application;
using Common.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Common.Infrastructure.Outbox;

internal class IdempotentDomainEventHandler<TEvent, TDbContext> : DomainEventHandler<TEvent>
    where TEvent : DomainEvent
    where TDbContext : DbContext
{
    private readonly TDbContext dbContext;
    private readonly IDomainEventHandler<TEvent> innerHandler;
    private readonly ILogger<IdempotentDomainEventHandler<TEvent, TDbContext>> logger;

    public IdempotentDomainEventHandler(
        TDbContext dbContext,
        IDomainEventHandler<TEvent> innerHandler,
        ILogger<IdempotentDomainEventHandler<TEvent, TDbContext>> logger)
    {
        this.dbContext = dbContext;
        this.innerHandler = innerHandler;
        this.logger = logger;
    }

    public override async Task Handle(TEvent domainEvent, CancellationToken cancellationToken)
    {
        var consumer = new OutboxMessageConsumer(domainEvent.Id, innerHandler.GetType().Name);

        var existingConsumer = await dbContext.Set<OutboxMessageConsumer>()
            .FirstOrDefaultAsync(x => x.OutboxMessageId == consumer.OutboxMessageId && x.Name == consumer.Name,
                                 cancellationToken);

        if (existingConsumer != null)
        {
            logger.LogInformation($"Event {domainEvent.Id} already processed.");
            return;
        }


        await innerHandler.Handle(domainEvent, cancellationToken);

        dbContext.Set<OutboxMessageConsumer>().Add(consumer);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
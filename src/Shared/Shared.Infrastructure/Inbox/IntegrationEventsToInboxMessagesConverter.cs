using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Application;

namespace Shared.Infrastructure.Inbox;

public class IntegrationEventsToInboxMessagesConverter<TEvent, TDbContext> : IConsumer<TEvent>
    where TEvent : IntegrationEvent
    where TDbContext : DbContext
{
    private readonly TDbContext dbContext;

    public IntegrationEventsToInboxMessagesConverter(TDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task Consume(ConsumeContext<TEvent> context)
    {
        var @event = context.Message;
        var inboxMessage = new InboxMessage(@event.GetType().AssemblyQualifiedName!, @event);
        dbContext.Set<InboxMessage>().Add(inboxMessage);
        return dbContext.SaveChangesAsync(context.CancellationToken);
    }
}

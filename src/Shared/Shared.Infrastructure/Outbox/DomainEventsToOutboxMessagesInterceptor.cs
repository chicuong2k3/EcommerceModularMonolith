using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared.Abstractions.Core;

namespace Shared.Infrastructure.Outbox;

public sealed class DomainEventsToOutboxMessagesInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;

        if (dbContext == null)
        {
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        var entities = dbContext.ChangeTracker.Entries<AggregateRoot>()
            .Select(x => x.Entity)
            .ToList();

        var outboxMessages = entities
            .SelectMany(x =>
            {
                var domainEvents = x.GetDomainEvents();
                return domainEvents;
            })
            .Select(e => new OutboxMessage(e.GetType().AssemblyQualifiedName!, e))
            .ToList();

        foreach (var entity in entities)
        {
            entity.ClearDomainEvents();
        }

        if (outboxMessages.Any())
        {
            await dbContext.Set<OutboxMessage>().AddRangeAsync(outboxMessages, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}

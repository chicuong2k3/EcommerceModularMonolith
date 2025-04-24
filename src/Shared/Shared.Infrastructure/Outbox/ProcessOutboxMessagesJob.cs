using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Reflection;

namespace Shared.Infrastructure.Outbox;

[DisallowConcurrentExecution]
public class ProcessOutboxMessagesJob<T> : IJob where T : DbContext
{
    private readonly T dbContext;
    private readonly IServiceProvider serviceProvider;
    private readonly Assembly[] handlerAssemblies;
    private readonly ILogger<ProcessOutboxMessagesJob<T>> logger;

    public ProcessOutboxMessagesJob(
        T dbContext,
        IServiceProvider serviceProvider,
        Assembly[] handlerAssemblies,
        ILogger<ProcessOutboxMessagesJob<T>> logger)
    {
        this.dbContext = dbContext;
        this.serviceProvider = serviceProvider;
        this.handlerAssemblies = handlerAssemblies;
        this.logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var messages = await dbContext.Set<OutboxMessage>()
                                .Where(x => x.ProcessedOn == null)
                                .Take(30)
                                .ToListAsync(context.CancellationToken);

        if (!messages.Any())
        {
            return;
        }
        using var transaction = await dbContext.Database.BeginTransactionAsync(context.CancellationToken);

        try
        {
            foreach (var message in messages)
            {
                await ProcessMessageAsync(message, context.CancellationToken);
            }

            await dbContext.SaveChangesAsync(context.CancellationToken);
            await transaction.CommitAsync(context.CancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(context.CancellationToken);
            throw;
        }
    }

    private async Task ProcessMessageAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        try
        {
            var domainEvent = message.DeserializeContent();
            var eventType = domainEvent.GetType();

            var handlers = DomainEventHandlerFactory.GetHandlers(eventType, serviceProvider, handlerAssemblies);
            foreach (var handler in handlers)
            {
                await handler.Handle(domainEvent, cancellationToken);
            }

            message.ProcessedOn = DateTime.UtcNow;
            logger.LogInformation("Processed outbox message with Id '{MessageId}'.", message.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process outbox message with Id '{MessageId}'.", message.Id);
            throw;
        }
    }
}

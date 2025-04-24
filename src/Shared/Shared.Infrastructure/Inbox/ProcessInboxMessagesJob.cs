using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Reflection;

namespace Shared.Infrastructure.Inbox;

[DisallowConcurrentExecution]
public class ProcessInboxMessagesJob<T> : IJob where T : DbContext
{
    private readonly T dbContext;
    private readonly IServiceProvider serviceProvider;
    private readonly Assembly[] handlerAssemblies;
    private readonly ILogger<ProcessInboxMessagesJob<T>> logger;

    public ProcessInboxMessagesJob(
        T dbContext,
        IServiceProvider serviceProvider,
        Assembly[] handlerAssemblies,
        ILogger<ProcessInboxMessagesJob<T>> logger)
    {
        this.dbContext = dbContext;
        this.serviceProvider = serviceProvider;
        this.handlerAssemblies = handlerAssemblies;
        this.logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var messages = await dbContext.Set<InboxMessage>()
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

    private async Task ProcessMessageAsync(InboxMessage message, CancellationToken cancellationToken)
    {
        try
        {
            var integrationEvent = message.DeserializeContent();
            var eventType = integrationEvent.GetType();

            var handlers = IntegrationEventHandlerFactory.GetHandlers(eventType, serviceProvider, handlerAssemblies);
            foreach (var handler in handlers)
            {
                await handler.Handle(integrationEvent, cancellationToken);
            }

            message.ProcessedOn = DateTime.UtcNow;
            logger.LogInformation("Processed inbox message with Id '{MessageId}'.", message.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process inbox message with Id '{MessageId}'.", message.Id);
            throw;
        }
    }
}

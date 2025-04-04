using Common.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Common.Infrastructure.Outbox;

[DisallowConcurrentExecution]
public class ProcessOutboxMessagesJob<T> : IJob where T : DbContext
{
    private readonly IPublisher publisher;
    private readonly T dbContext;
    private readonly ILogger<ProcessOutboxMessagesJob<T>> logger;

    public ProcessOutboxMessagesJob(
        IPublisher publisher,
        T dbContext,
        ILogger<ProcessOutboxMessagesJob<T>> logger)
    {
        this.publisher = publisher;
        this.dbContext = dbContext;
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
                try
                {
                    var @event = message.DeserializeContent();

                    await publisher.Publish(@event, context.CancellationToken);

                    message.ProcessedOn = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    logger.LogError($"Failed to process message with id '{message.Id}'. Error: {ex.Message}");
                }
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
}

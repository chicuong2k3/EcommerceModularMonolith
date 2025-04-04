using Common.Messages;

namespace Common.Application;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
        where T : IntegrationEvent;
}

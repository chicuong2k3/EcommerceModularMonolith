using MassTransit;
using Shared.Abstractions.Application;

namespace Shared.Infrastructure.Messaging;

internal class EventBus : IEventBus
{
    private readonly IBus bus;

    public EventBus(IBus bus)
    {
        this.bus = bus;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent
    {
        await bus.Publish(@event, cancellationToken);
    }
}

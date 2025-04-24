namespace Shared.Abstractions.Application;

public interface IIntegrationEventHandler<TEvent> : IIntegrationEventHandler
    where TEvent : IntegrationEvent
{
    Task Handle(TEvent @event, CancellationToken cancellationToken = default);
}

public interface IIntegrationEventHandler
{
    Task Handle(IntegrationEvent @event, CancellationToken cancellationToken = default);
}
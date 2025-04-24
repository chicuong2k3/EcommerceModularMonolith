using Shared.Abstractions.Core;

namespace Shared.Abstractions.Application;

public interface IDomainEventHandler<TEvent> : IDomainEventHandler
    where TEvent : DomainEvent
{
    Task Handle(TEvent domainEvent, CancellationToken cancellationToken = default);
}

public interface IDomainEventHandler
{
    Task Handle(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}

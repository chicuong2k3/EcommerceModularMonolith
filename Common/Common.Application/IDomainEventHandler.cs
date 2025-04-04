using MediatR;

namespace Common.Domain;

public interface IDomainEventHandler<TEvent> : INotificationHandler<TEvent>
    where TEvent : DomainEvent
{
}

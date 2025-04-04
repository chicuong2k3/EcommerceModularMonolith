using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Domain;

public abstract class AggregateRoot : Entity
{
    [NotMapped]
    private readonly List<DomainEvent> domainEvents = [];

    public IReadOnlyCollection<DomainEvent> GetDomainEvents()
    {
        return [.. domainEvents];
    }

    public void ClearDomainEvents()
    {
        domainEvents.Clear();
    }

    protected void Raise(DomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }
}

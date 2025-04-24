using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Shared.Abstractions.Core;

public abstract class AggregateRoot
{
    [JsonInclude]
    public Guid Id { get; protected set; }

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

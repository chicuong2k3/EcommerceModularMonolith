namespace Catalog.Domain.ProductAggregate.Events;

public record ProductCreated(Guid ProductId, string Name) : DomainEvent;

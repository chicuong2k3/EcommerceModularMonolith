namespace Catalog.Domain.ProductAggregate.Events;

public record ProductVariantQuantityUpdated(Guid ProductId, Guid ProductVariantId, int NewQuantity) : DomainEvent;

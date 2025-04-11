namespace Catalog.Domain.ProductAggregate.Events;

public record ProductVariantAdded(Guid ProductId,
                                  string ProductName,
                                  Guid ProductVariantId,
                                  decimal OriginalPrice,
                                  decimal? SalePrice,
                                  int Quantity,
                                  string? ImageUrl,
                                  Dictionary<string, string> Attributes) : DomainEvent;
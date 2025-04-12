namespace Ordering.Application.Products.Commands;

public record DeleteProduct(
    Guid ProductId,
    Guid ProductVariantId) : ICommand;

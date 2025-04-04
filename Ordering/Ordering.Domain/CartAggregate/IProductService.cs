namespace Ordering.Domain.CartAggregate;

public interface IProductService
{
    Task<Result> ValidateProductAvailabilityAsync(Guid productId, Guid productVariantId, int quantity);
}

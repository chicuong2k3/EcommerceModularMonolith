using FluentResults;

namespace Ordering.Infrastructure.Services;

internal class ProuductService : IProductService
{
    public async Task<Result> ValidateProductAvailabilityAsync(Guid productId, Guid productVariantId, int quantity)
    {
        return Result.Ok();
    }
}

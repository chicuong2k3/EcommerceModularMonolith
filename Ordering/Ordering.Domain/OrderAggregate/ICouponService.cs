namespace Ordering.Domain.OrderAggregate;

public interface ICouponService
{
    Task<Money?> ApplyCouponAsync(OrderDetails orderDetails, CancellationToken cancellationToken = default);
}

namespace Promotion.Domain.CouponAggregate;

public interface ICouponRepository
{
    Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<List<Coupon>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Coupon coupon, CancellationToken cancellationToken = default);
    Task RemoveAsync(Coupon coupon, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

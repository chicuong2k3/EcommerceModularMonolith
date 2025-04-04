
namespace Promotion.Infrastructure.Persistence.Repositories;

internal class CouponRepository : ICouponRepository
{
    private readonly PromotionDbContext dbContext;

    public CouponRepository(PromotionDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task AddAsync(Coupon coupon, CancellationToken cancellationToken = default)
    {
        dbContext.Coupons.Add(coupon);
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<List<Coupon>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Coupons
            .Include(c => c.Discount)
            .Include(c => c.Conditions)
            .ToListAsync(cancellationToken);
    }

    public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await dbContext.Coupons.Where(c => c.Code == code)
                            .Include(c => c.Discount)
                            .Include(c => c.Conditions)
                            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task RemoveAsync(Coupon coupon, CancellationToken cancellationToken = default)
    {
        dbContext.Coupons.Remove(coupon);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;
using Pay.Core.Entities;
using Pay.Core.Repositories;

namespace Pay.Infrastructure.Persistence.Repositories;

internal class PaymentRepository : IPaymentRepository
{
    private readonly PayDbContext dbContext;

    public PaymentRepository(PayDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        dbContext.Add(payment);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Payment?> GetPaymentByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Payments
            .Include(p => p.Transactions)
            .FirstOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

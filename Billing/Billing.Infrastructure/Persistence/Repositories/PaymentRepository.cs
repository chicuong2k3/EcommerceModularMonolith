using Billing.Domain.PaymentAggregate;

namespace Billing.Infrastructure.Persistence.Repositories;

internal class PaymentRepository : IPaymentRepository
{
    private readonly BillingDbContext dbContext;

    public PaymentRepository(BillingDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        dbContext.Add(payment);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

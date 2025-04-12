namespace Billing.Domain.PaymentAggregate;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<Payment?> GetPaymentByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}

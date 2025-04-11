namespace Billing.Domain.PaymentAggregate;

public interface IPaymentGateway
{
    string Reference { get; }

    Task<Result<PaymentUrlInfo>> CreatePaymentUrlAsync(
        Payment payment,
        string returnUrl,
        CancellationToken cancellationToken = default);

    Task<Result> AuthorizeAsync(
        Payment payment,
        Transaction transaction,
        CancellationToken cancellationToken = default);

    Task<Result> RefundAsync(
        Payment payment,
        Transaction transaction,
        CancellationToken cancellationToken = default);
}

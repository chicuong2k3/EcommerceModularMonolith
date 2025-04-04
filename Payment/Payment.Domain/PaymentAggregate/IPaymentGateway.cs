namespace Payment.Domain.PaymentAggregate;

public interface IPaymentGateway
{
    string Reference { get; }
    Task<Result> AuthorizeAsync(Payment payment, Transaction transaction, CancellationToken cancellationToken = default);
}

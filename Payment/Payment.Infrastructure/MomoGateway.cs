using FluentResults;
using Payment.Domain.PaymentAggregate;

namespace Payment.Infrastructure;

internal class MomoGateway : IPaymentGateway
{
    public string Reference => "MOMO_GATEWAY";

    public Task<Result> AuthorizeAsync(Domain.PaymentAggregate.Payment payment, Transaction transaction, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Ok());
    }
}

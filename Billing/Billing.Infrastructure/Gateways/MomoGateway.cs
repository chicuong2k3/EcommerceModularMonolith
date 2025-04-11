using FluentResults;

namespace Billing.Infrastructure.Gateways;

internal class MomoGateway : IPaymentGateway
{
    public string Reference => "MOMO_GATEWAY";

    public Task<Result> AuthorizeAsync(Payment payment, Transaction transaction, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Ok());
    }

    public Task<Result> RefundAsync(Payment payment, Transaction transaction, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

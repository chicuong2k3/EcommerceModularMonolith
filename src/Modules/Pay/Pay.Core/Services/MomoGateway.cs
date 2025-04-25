using FluentResults;
using Pay.Core.Entities;
using Pay.Core.ValueObjects;

namespace Pay.Core.Services;

internal class MomoGateway : IPaymentGateway
{
    public string Reference => "MOMO_GATEWAY";

    public Result<PaymentUrlInfo> CreatePaymentUrl(Payment payment)
    {
        throw new NotImplementedException();
    }

    public Task<Result> RefundAsync(Payment payment, decimal refundAmount, string refundReason, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

using FluentResults;
using Pay.Core.Entities;
using Pay.Core.ValueObjects;

namespace Pay.Core.Services;

public interface IPaymentGateway
{
    string Reference { get; }

    Result<PaymentUrlInfo> CreatePaymentUrl(
        Payment payment);

    Task<Result> RefundAsync(
        Payment payment,
        decimal refundAmount,
        string refundReason,
        CancellationToken cancellationToken = default);
}

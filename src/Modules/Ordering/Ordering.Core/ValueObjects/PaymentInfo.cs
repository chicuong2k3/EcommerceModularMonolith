using FluentResults;
using Shared.Abstractions.Core;

namespace Ordering.Core.ValueObjects;

public record PaymentInfo
{
    public PaymentMethod PaymentMethod { get; private set; }

    private PaymentInfo() { }

    private PaymentInfo(PaymentMethod paymentMethod)
    {
        PaymentMethod = paymentMethod;
    }

    public static Result<PaymentInfo> Create(string paymentMethod)
    {
        if (!Enum.TryParse<PaymentMethod>(paymentMethod, out var parsedPaymentMethod))
            return Result.Fail(new ValidationError("Invalid payment method"));

        return Result.Ok(new PaymentInfo(parsedPaymentMethod));
    }
}

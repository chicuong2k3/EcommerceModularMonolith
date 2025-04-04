namespace Ordering.Domain.OrderAggregate;

public record PaymentInfo
{
    /// <summary>
    /// The accepted payment options, such as VISA, Mastercard, or digital wallets.
    /// </summary>
    public string PaymentMethod { get; private set; }

    private PaymentInfo() { }

    private PaymentInfo(string paymentMethod)
    {
        PaymentMethod = paymentMethod;
    }

    public static Result<PaymentInfo> Create(string paymentMethod)
    {
        if (string.IsNullOrWhiteSpace(paymentMethod))
            return Result.Fail(new ValidationError("Payment method is required"));

        return Result.Ok(new PaymentInfo(paymentMethod));
    }
}

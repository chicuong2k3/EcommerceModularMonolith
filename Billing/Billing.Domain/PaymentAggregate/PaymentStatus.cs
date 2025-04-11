namespace Billing.Domain.PaymentAggregate;

public enum PaymentStatus
{
    Pending,
    UrlGenerated,
    Succeeded,
    Failed,
    Canceled,
    Refunded
}

using FluentResults;
using Pay.Core.Events;
using Pay.Core.Services;
using Pay.Core.ValueObjects;
using Shared.Abstractions.Core;

namespace Pay.Core.Entities;

public class Payment : AggregateRoot
{
    public Guid OrderId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Money TotalAmount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? PaymentUrl { get; private set; }
    public string? PaymentToken { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ExpiryTime { get; private set; }
    private List<Transaction> transactions = new();
    public IReadOnlyCollection<Transaction> Transactions => transactions.AsReadOnly();

    private Payment() { }

    public Payment(
        Guid orderId,
        Guid customerId,
        Money totalAmount)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        ExpiryTime = CreatedAt.AddMinutes(15);
    }

    public void SetPaymentUrlAndToken(string paymentUrl, string paymentToken)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Can only set payment URL for pending payments");

        PaymentUrl = paymentUrl;
        PaymentToken = paymentToken;
        Status = PaymentStatus.UrlGenerated;
    }

    public async Task<Result> AuthorizeAsync(IPaymentGateway gateway, CancellationToken cancellationToken = default)
    {
        if (Status != PaymentStatus.UrlGenerated)
            return Result.Fail("Payment must have URL generated before authorization");

        if (DateTime.UtcNow > ExpiryTime)
            return Result.Fail("Payment URL has expired");

        var transaction = new Transaction(TotalAmount, gateway.Reference);
        transactions.Add(transaction);

        var result = await gateway.AuthorizeAsync(this, transaction, cancellationToken);
        if (result.IsSuccess)
        {
            transaction.Status = TransactionStatus.Succeeded;
            if (transactions.Sum(t => t.Amount) >= TotalAmount)
            {
                Status = PaymentStatus.Succeeded;
                Raise(new PaymentSucceeded(OrderId));
            }
        }
        else
        {
            transaction.Status = TransactionStatus.Failed;
            Status = PaymentStatus.Failed;
            Raise(new PaymentFailed(OrderId, result.Errors.Select(e => e.Message)));
        }

        return result;
    }

    public Result ProcessCallback(string paymentToken, string transactionId, string status)
    {
        if (PaymentToken != paymentToken)
            return Result.Fail("Invalid payment token");

        if (DateTime.UtcNow > ExpiryTime)
            return Result.Fail("Payment URL has expired");

        switch (status.ToLower())
        {
            case "success":
                Status = PaymentStatus.Succeeded;
                Raise(new PaymentSucceeded(OrderId));
                break;
            case "failed":
                Status = PaymentStatus.Failed;
                Raise(new PaymentFailed(OrderId, new[] { "Payment failed" }));
                break;
            default:
                return Result.Fail("Invalid payment status");
        }

        return Result.Ok();
    }

    public Result Cancel()
    {
        if (Status != PaymentStatus.Pending && Status != PaymentStatus.UrlGenerated)
            return Result.Fail("Can only cancel pending payments");

        Status = PaymentStatus.Canceled;
        //Raise(new PaymentCanceled(OrderId));
        return Result.Ok();
    }

    public async Task<Result> RefundAsync(IPaymentGateway gateway, Money amount, CancellationToken cancellationToken = default)
    {
        if (Status != PaymentStatus.Succeeded)
            return Result.Fail("Only succeeded payments can be refunded");

        var refundTransaction = new Transaction(amount, gateway.Reference);
        var result = await gateway.RefundAsync(this, refundTransaction, cancellationToken);
        if (result.IsSuccess)
        {
            Status = PaymentStatus.Refunded;
            refundTransaction.Status = TransactionStatus.Succeeded;
            //Raise(new PaymentRefunded(OrderId, amount));
        }

        return result;
    }
}

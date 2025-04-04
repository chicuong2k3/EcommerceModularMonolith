namespace Payment.Domain.PaymentAggregate;

public class Payment : AggregateRoot
{
    public Money TotalAmount { get; private set; }
    public string? Details { get; private set; }
    public PaymentStatus Status { get; private set; }
    private List<Transaction> transactions = new();
    public IReadOnlyCollection<Transaction> Transactions => transactions.AsReadOnly();
    public DateTime CreatedAt { get; }

    private Payment() { }

    private Payment(
        Money totalAmount,
        string? details)
    {
        Id = Guid.NewGuid();
        TotalAmount = totalAmount;
        Details = details;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<Payment> Create(Money totalAmount, string? details)
    {
        return Result.Ok(new Payment(totalAmount, details));
    }


    public async Task<Result> AuthorizeAsync(IPaymentGateway gateway, CancellationToken cancellationToken = default)
    {
        if (Status != PaymentStatus.Pending)
            return Result.Fail("Payment is not pending");

        var transaction = new Transaction(TotalAmount, gateway.Reference);
        transactions.Add(transaction);

        var result = await gateway.AuthorizeAsync(this, transaction, cancellationToken);
        if (result.IsSuccess)
        {
            Status = PaymentStatus.Succeeded;
            transaction.UpdateStatus(TransactionStatus.Succeeded);
            //Raise(new PaymentSucceeded(Id));
        }
        else
        {
            Status = PaymentStatus.Failed;
            transaction.UpdateStatus(TransactionStatus.Failed);
            //Raise(new PaymentFailed(Id, result.Error));
        }

        return result;
    }
}

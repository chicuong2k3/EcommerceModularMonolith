namespace Payment.Domain.PaymentAggregate;

public class Transaction : Entity
{
    public Money Amount { get; private set; }
    public TransactionStatus Status { get; private set; }
    public DateTime CreatedAt { get; }
    public string GatewayReference { get; private set; }

    internal Transaction(Money amount, string gatewayReference)
    {
        Id = Guid.NewGuid();
        Amount = amount;
        GatewayReference = gatewayReference;
        Status = TransactionStatus.Pending;
    }

    public void UpdateStatus(TransactionStatus newStatus)
    {
        Status = newStatus;
    }
}

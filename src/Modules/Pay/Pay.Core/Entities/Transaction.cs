using Pay.Core.ValueObjects;

namespace Pay.Core.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public Money Amount { get; private set; }
    public TransactionStatus Status { get; set; }
    public DateTime CreatedAt { get; }
    public string GatewayReference { get; private set; }

    internal Transaction(Money amount, string gatewayReference)
    {
        Id = Guid.NewGuid();
        Amount = amount;
        GatewayReference = gatewayReference;
        Status = TransactionStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }
}

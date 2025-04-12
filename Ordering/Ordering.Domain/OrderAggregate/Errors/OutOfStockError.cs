namespace Ordering.Domain.OrderAggregate.Errors;

public class OutOfStockError : Error
{
    public OutOfStockError(string productName, int currentQuantity, int requestedQuantity)
        : base($"Product '{productName}' is out of stock. Current quantity: {currentQuantity} .Requested quantity: {requestedQuantity}.")
    {
    }
}

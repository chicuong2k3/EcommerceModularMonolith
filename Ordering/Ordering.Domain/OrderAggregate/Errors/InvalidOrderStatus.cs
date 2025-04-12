namespace Ordering.Domain.OrderAggregate.Errors;

public class InvalidOrderStatus : Error
{
    public InvalidOrderStatus(string message) : base(message)
    {
    }
}

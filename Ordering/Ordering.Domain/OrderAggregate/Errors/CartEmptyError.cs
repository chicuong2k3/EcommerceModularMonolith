namespace Ordering.Domain.OrderAggregate.Errors;

public class CartEmptyError : Error
{
    public CartEmptyError(string message) : base(message)
    {
    }
}

namespace Promotion.Domain.Common;

public class OrderDetails
{
    public Guid OrderId { get; set; }
    public Money Subtotal { get; set; }
}
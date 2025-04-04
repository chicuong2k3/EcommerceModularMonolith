namespace Ordering.Domain.OrderAggregate;

public class Order : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    private List<OrderItem> items = [];
    public IReadOnlyCollection<OrderItem> Items => items.AsReadOnly();
    public OrderStatus Status { get; private set; }

    public DateTime OrderDate { get; private set; }
    public Money Total { get; private set; }
    public Money Subtotal { get; private set; }

    public PaymentInfo PaymentInfo { get; set; }
    public ShippingInfo ShippingInfo { get; set; }

    private Order()
    {
        items = new List<OrderItem>();
    }

    private Order(
        Guid customerId,
        PaymentInfo paymentInfo,
        ShippingInfo shippingInfo,
        List<OrderItem> orderItems)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        PaymentInfo = paymentInfo;
        ShippingInfo = shippingInfo;
        OrderDate = DateTime.UtcNow;
        items.AddRange(orderItems);
        Subtotal = items.Aggregate(Money.FromDecimal(0).Value, (sum, item) => sum + (item.SalePrice * item.Quantity));
    }

    public static Result<Order> Create(
        Guid customerId,
        PaymentInfo paymentInfo,
        ShippingInfo shippingInfo,
        List<OrderItem> orderItems)
    {
        if (orderItems.Any(oi => oi.Quantity <= 0))
            return Result.Fail(new ValidationError("Order item quantity must be greater than 0"));

        var order = new Order(customerId, paymentInfo, shippingInfo, orderItems);

        return Result.Ok(order);
    }

    public Result Place(Money? discountAmount)
    {
        if (!Items.Any())
            return Result.Fail(new ValidationError("Order must have at least one item to be placed"));

        if (discountAmount != null)
        {
            Total = Subtotal - discountAmount;
        }
        else
        {
            Total = Subtotal;
        }

        Status = OrderStatus.Placed;

        Raise(new OrderPlaced(Id, CustomerId, Items));
        return Result.Ok();
    }


    public Result StartProcessing()
    {
        if (Status != OrderStatus.Placed)
            return Result.Fail(new ValidationError("Order must be Placed before processing"));

        Status = OrderStatus.Processing;
        //Raise(new OrderProcessingStarted(Id));
        return Result.Ok();
    }

    public Result MarkAsShipped(string trackingNumber)
    {
        if (Status != OrderStatus.Processing)
            return Result.Fail(new ValidationError("Order must be Processing before shipping"));

        Status = OrderStatus.Shipped;
        //Raise(new OrderShipped(Id, trackingNumber));
        return Result.Ok();
    }

    public Result MarkAsDelivered()
    {
        if (Status != OrderStatus.Shipped)
            return Result.Fail(new ValidationError("Order must be Shipped before delivery"));

        Status = OrderStatus.Delivered;
        //Raise(new OrderDelivered(Id));
        return Result.Ok();
    }

    public Result Cancel()
    {
        if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered || Status == OrderStatus.Refunded)
            return Result.Fail(new ValidationError("Cannot cancel a shipped/delivered/refunded order"));

        Status = OrderStatus.Canceled;
        Raise(new OrderCanceled(Id));
        return Result.Ok();
    }

    public Result Refund(decimal amount)
    {
        if (Status != OrderStatus.Delivered)
            return Result.Fail(new ValidationError("Refunds are only allowed for delivered orders"));

        Status = OrderStatus.Refunded;
        //Raise(new OrderRefunded(Id, amount));
        return Result.Ok();
    }
}

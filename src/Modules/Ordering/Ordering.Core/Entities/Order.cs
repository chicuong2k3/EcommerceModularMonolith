using FluentResults;
using Ordering.Core.Events;
using Ordering.Core.ValueObjects;
using Shared.Abstractions.Core;

namespace Ordering.Core.Entities;

public class Order : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    private List<OrderItem> items = [];
    public IReadOnlyCollection<OrderItem> Items => items.AsReadOnly();
    public OrderStatus Status { get; set; }

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
        Guid id,
        Guid customerId,
        PaymentInfo paymentInfo,
        ShippingInfo shippingInfo,
        List<OrderItem> orderItems)
    {
        Id = id;
        CustomerId = customerId;
        PaymentInfo = paymentInfo;
        ShippingInfo = shippingInfo;
        OrderDate = DateTime.UtcNow;
        items.AddRange(orderItems);
        Subtotal = items.Aggregate(Money.FromDecimal(0).Value, (sum, item) => sum + item.SalePrice * item.Quantity);

        Status = paymentInfo.PaymentMethod == PaymentMethod.COD
                    ? OrderStatus.Processing
                    : OrderStatus.PendingPayment;
    }

    public static Result<Order> Create(
        Guid id,
        Guid customerId,
        PaymentInfo paymentInfo,
        ShippingInfo shippingInfo,
        List<OrderItem> orderItems)
    {
        if (id == Guid.Empty)
            return Result.Fail(new ValidationError("Id is required."));
        if (!orderItems.Any())
            return Result.Fail("Order must have at least one item");

        var order = new Order(id, customerId, paymentInfo, shippingInfo, orderItems);
        order.Total = order.Subtotal + order.ShippingInfo.ShippingCosts;

        return Result.Ok(order);
    }

    public void RaisePaymentEvent()
    {
        switch (PaymentInfo.PaymentMethod)
        {
            case PaymentMethod.VNPay:
            case PaymentMethod.MoMo:
                Raise(new OrderPlacedForOnlinePayment(
                    Id,
                    CustomerId,
                    Total,
                    PaymentInfo.PaymentMethod.ToString()));
                break;
        }
    }

    public Result MarkAsPaid()
    {
        if (Status != OrderStatus.PendingPayment)
            return Result.Fail(new Error("Order status must be PendingPayment to be marked as paid"));

        Status = OrderStatus.Paid;
        return Result.Ok();
    }

    public Result StartProcessing()
    {
        if (PaymentInfo.PaymentMethod != PaymentMethod.COD && Status != OrderStatus.Paid)
            return Result.Fail(new Error("Online payment order must be paid before starting processing"));
        if (Status != OrderStatus.PendingPayment && Status != OrderStatus.Paid)
            return Result.Fail(new Error("Order status must be PendingPayment or Paid to start processing"));

        Status = OrderStatus.Processing;
        return Result.Ok();
    }

    public Result MarkAsShipped(string trackingNumber)
    {
        if (Status != OrderStatus.Processing)
            return Result.Fail(new Error("Order must be Processing before shipping"));

        Status = OrderStatus.Shipped;
        //Raise(new OrderShipped(Id, trackingNumber));
        return Result.Ok();
    }

    public Result MarkAsDelivered()
    {
        if (Status != OrderStatus.Shipped)
            return Result.Fail(new Error("Order must be Shipped before delivery"));

        Status = OrderStatus.Delivered;
        return Result.Ok();
    }

    public Result Cancel()
    {
        if (Status == OrderStatus.Shipped
            || Status == OrderStatus.Delivered
            || Status == OrderStatus.Canceled
            || Status == OrderStatus.Refunded)
            return Result.Fail(new Error("Cannot cancel a shipped/delivered/canceled/refunded order"));

        Status = OrderStatus.Canceled;
        Raise(new OrderCanceled(Id));
        return Result.Ok();
    }

    public Result Refund(decimal amount)
    {
        if (Status != OrderStatus.Delivered)
            return Result.Fail(new Error("Refunds are only allowed for delivered orders"));

        Status = OrderStatus.Refunded;
        //Raise(new OrderRefunded(Id, amount));
        return Result.Ok();
    }
}

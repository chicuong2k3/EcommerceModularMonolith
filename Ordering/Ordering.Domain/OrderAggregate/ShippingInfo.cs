namespace Ordering.Domain.OrderAggregate;

public record ShippingInfo
{
    public Money ShippingCosts { get; private set; }
    public Location ShippingAddress { get; private set; }

    private ShippingInfo() { }

    public ShippingInfo(Money shippingCosts, Location shippingAddress)
    {
        ShippingCosts = shippingCosts;
        ShippingAddress = shippingAddress;
    }
}

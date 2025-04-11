namespace Ordering.Domain.OrderAggregate;

public record ShippingInfo
{
    public Money ShippingCosts { get; private set; }
    public Location ShippingAddress { get; private set; }
    public string PhoneNumber { get; private set; }

    private ShippingInfo() { }

    private ShippingInfo(Money shippingCosts, Location shippingAddress, string phoneNumber)
    {
        ShippingCosts = shippingCosts;
        ShippingAddress = shippingAddress;
        PhoneNumber = phoneNumber;
    }

    public static Result<ShippingInfo> Create(Money shippingCosts, Location shippingAddress, string phoneNumber)
    {
        if (phoneNumber.Length < 10)
            return Result.Fail(new ValidationError("Phone number must be at least 10 digits long."));

        return Result.Ok(new ShippingInfo(shippingCosts, shippingAddress, phoneNumber));
    }
}

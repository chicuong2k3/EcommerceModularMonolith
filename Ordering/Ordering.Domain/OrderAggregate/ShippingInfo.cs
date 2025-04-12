using System.Text.RegularExpressions;

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
        if (!IsValidPhoneNumber(phoneNumber))
            return Result.Fail(new ValidationError("Phone number is invalid."));

        return Result.Ok(new ShippingInfo(shippingCosts, shippingAddress, phoneNumber));
    }

    static bool IsValidPhoneNumber(string phoneNumber)
    {
        // Define the regex pattern for phone number validation
        string pattern = @"^(\+?\d{1,3}[-.\s]?)?(\(?\d{1,4}\)?[-.\s]?)?[\d\s.-]{5,15}$";

        Regex regex = new Regex(pattern);

        return regex.IsMatch(phoneNumber);
    }
}

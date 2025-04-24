using FluentResults;
using Shared.Abstractions.Core;
using System.Text.RegularExpressions;

namespace Ordering.Core.ValueObjects;

public record ShippingInfo
{
    public Money ShippingCosts { get; private set; }
    public Location ShippingAddress { get; private set; }
    public string PhoneNumber { get; private set; }
    public ShippingMethod ShippingMethod { get; private set; }

    private ShippingInfo() { }

    private ShippingInfo(
        Location shippingAddress,
        string phoneNumber,
        ShippingMethod shippingMethod)
    {
        ShippingAddress = shippingAddress;
        PhoneNumber = phoneNumber;
        ShippingMethod = shippingMethod;
    }

    public static Result<ShippingInfo> Create(Location shippingAddress,
                                              string phoneNumber,
                                              ShippingMethod shippingMethod)
    {
        if (!IsValidPhoneNumber(phoneNumber))
            return Result.Fail(new ValidationError("Phone number is invalid."));

        var shippingInfo = new ShippingInfo(shippingAddress, phoneNumber, shippingMethod);

        // Calculate shipping costs based on the shipping method and address
        shippingInfo.ShippingCosts = Money.FromDecimal(0).Value;

        return Result.Ok(shippingInfo);
    }

    static bool IsValidPhoneNumber(string phoneNumber)
    {
        // Define the regex pattern for phone number validation
        string pattern = @"^(\+?\d{1,3}[-.\s]?)?(\(?\d{1,4}\)?[-.\s]?)?[\d\s.-]{5,15}$";

        Regex regex = new Regex(pattern);

        return regex.IsMatch(phoneNumber);
    }
}

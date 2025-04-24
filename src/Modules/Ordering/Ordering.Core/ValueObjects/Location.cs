using FluentResults;
using Shared.Abstractions.Core;

namespace Ordering.Core.ValueObjects;

public record Location
{
    public string? Street { get; private set; }
    public string Ward { get; private set; }
    public string District { get; private set; }
    public string Province { get; private set; }
    public string Country { get; private set; }

    private Location()
    {
    }

    private Location(string? street,
                    string ward,
                    string district,
                    string province,
                    string country)
    {
        Street = street;
        Ward = ward;
        District = district;
        Province = province;
        Country = country;
    }

    public static Result<Location> Create(string? street, string ward, string district, string province, string country)
    {
        if (string.IsNullOrWhiteSpace(ward))
            return Result.Fail(new ValidationError("Ward is required."));
        if (string.IsNullOrWhiteSpace(district))
            return Result.Fail(new ValidationError("District is required."));
        if (string.IsNullOrWhiteSpace(province))
            return Result.Fail(new ValidationError("Province is required."));
        if (string.IsNullOrWhiteSpace(country))
            return Result.Fail(new ValidationError("Country is required."));

        return Result.Ok(new Location(street, ward, district, province, country));
    }
}

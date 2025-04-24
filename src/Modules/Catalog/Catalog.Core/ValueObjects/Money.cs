using FluentResults;
using Shared.Abstractions.Core;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Catalog.Core.ValueObjects;

public record Money
{
    [JsonInclude]
    public decimal Amount { get; private set; }


    public Money() { }

    private Money(decimal amount)
    {
        Amount = amount;
    }

    public static Result<Money> FromDecimal(decimal amount)
    {
        if (amount < 0)
            return Result.Fail(new ValidationError("Money amount cannot be negative"));

        return Result.Ok(new Money(amount));
    }

    public static Result<Money> FromString(string amount)
    {
        if (!decimal.TryParse(amount, NumberStyles.Currency, CultureInfo.InvariantCulture, out var parsedAmount))
        {
            return Result.Fail(new ValidationError("Invalid amount."));
        }

        if (parsedAmount < 0)
            return Result.Fail(new ValidationError("Money amount cannot be negative"));

        return Result.Ok(new Money(parsedAmount));
    }

    public static Money operator +(Money left, Money right)
    {
        return left with { Amount = left.Amount + right.Amount };
    }

    public static Money operator -(Money left, Money right)
    {
        return left with { Amount = left.Amount - right.Amount };
    }

    public static Money operator *(Money left, decimal right)
    {
        return left with { Amount = left.Amount * right };
    }

    public static implicit operator decimal(Money money) => money.Amount;

    public static bool operator >(Money left, Money right)
    {
        return left.Amount > right.Amount;
    }

    public static bool operator <(Money left, Money right)
    {
        return left.Amount < right.Amount;
    }
}
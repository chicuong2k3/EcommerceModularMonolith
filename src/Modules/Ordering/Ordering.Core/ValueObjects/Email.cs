using FluentResults;
using Shared.Abstractions.Core;

namespace Ordering.Core.ValueObjects;

public record Email
{
    public string Value { get; private set; }

    private Email() { }

    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Fail(new ValidationError("Email cannot be empty"));
        }

        if (!IsValidEmail(value))
        {
            return Result.Fail(new ValidationError("Invalid email format"));
        }

        return Result.Ok(new Email(value));
    }

    private static bool IsValidEmail(string email)
    {
        return true;
    }
}

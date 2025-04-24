using FluentResults;
using Shared.Abstractions.Core;

namespace Catalog.Core.ValueObjects;

public record ReviewRating
{
    public int Value { get; init; }

    private ReviewRating(int value)
    {
        Value = value;
    }

    public static Result<ReviewRating> Create(int value)
    {
        if (value < 1 || value > 5)
        {
            return Result.Fail(new ValidationError("Rating must be between 1 and 5."));
        }

        return Result.Ok(new ReviewRating(value));
    }

    public static implicit operator int(ReviewRating rating) => rating.Value;
}

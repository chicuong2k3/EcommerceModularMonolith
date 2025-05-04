using FluentResults;
using Shared.Abstractions.Core;

namespace Catalog.Core.ValueObjects;

public record DateTimeRange
{
    public DateTime Start { get; }
    public DateTime End { get; }

    private DateTimeRange(DateTime start, DateTime end)
    {
        Start = start.ToUniversalTime();
        End = end.ToUniversalTime();
    }

    public static Result<DateTimeRange> Create(DateTime start, DateTime end)
    {
        if (start > end)
            return Result.Fail(new ValidationError("Start date must be less than end date."));

        return Result.Ok(new DateTimeRange(start, end));
    }
}

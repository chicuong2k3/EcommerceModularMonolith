using FluentResults;

namespace Shared.Abstractions.Core;

public class NotFoundError : Error
{
    public NotFoundError(string message) : base(message)
    {
    }
}
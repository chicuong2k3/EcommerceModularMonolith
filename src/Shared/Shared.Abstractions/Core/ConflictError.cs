using FluentResults;

namespace Shared.Abstractions.Core;

public class ConflictError : Error
{
    public ConflictError(string message) : base(message)
    {
    }
}

using FluentResults;

namespace Shared.Abstractions.Core;

public class ValidationError : Error
{
    public ValidationError(string message) : base(message)
    {
    }
}
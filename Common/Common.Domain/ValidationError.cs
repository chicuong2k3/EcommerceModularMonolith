using FluentResults;

namespace Common.Domain;

public class ValidationError : Error
{
    public ValidationError(string message) : base(message)
    {
    }
}
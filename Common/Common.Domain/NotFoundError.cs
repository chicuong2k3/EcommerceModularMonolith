using FluentResults;

namespace Common.Domain;

public class NotFoundError : Error
{
    public NotFoundError(string message) : base(message)
    {
    }
}
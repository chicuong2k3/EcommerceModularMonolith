using FluentResults;
using MediatR;

namespace Shared.Abstractions.Application;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<T> : IRequest<Result<T>>
{
}

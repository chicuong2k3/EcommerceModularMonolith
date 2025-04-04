using FluentResults;
using MediatR;

namespace Common.Application;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<T> : IRequest<Result<T>>
{
}

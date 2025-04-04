using FluentResults;
using MediatR;

namespace Common.Application;

public interface IQuery<T> : IRequest<Result<T>>
{
}

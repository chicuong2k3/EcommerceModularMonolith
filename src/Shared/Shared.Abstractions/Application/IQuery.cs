using FluentResults;
using MediatR;

namespace Shared.Abstractions.Application;

public interface IQuery<T> : IRequest<Result<T>>
{
}

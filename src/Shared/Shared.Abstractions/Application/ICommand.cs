using FluentResults;
using MediatR;

namespace Shared.Abstractions.Application;

public interface ICommand : IRequest<Result>
{
}
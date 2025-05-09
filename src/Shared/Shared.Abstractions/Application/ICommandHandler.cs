﻿using FluentResults;
using MediatR;

namespace Shared.Abstractions.Application;

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
}
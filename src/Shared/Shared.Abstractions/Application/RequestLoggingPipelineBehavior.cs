using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Diagnostics;

namespace Shared.Abstractions.Application;

public class RequestLoggingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : IResultBase
{
    private readonly ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger;

    public RequestLoggingPipelineBehavior(ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        this.logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request,
                                  RequestHandlerDelegate<TResponse> next,
                                  CancellationToken cancellationToken)
    {
        var moduleName = typeof(TRequest).FullName?.Split('.').LastOrDefault();
        var requestName = typeof(TRequest).Name;

        Activity.Current?.SetTag("request.module", moduleName);
        Activity.Current?.SetTag("request.name", requestName);

        using (LogContext.PushProperty("Module", moduleName))
        {
            logger.LogInformation("Handling request {RequestName}", requestName);

            TResponse response = await next();

            if (response.IsSuccess)
            {
                logger.LogInformation("Request {RequestName} handled successfully", requestName);
            }
            else
            {
                using (LogContext.PushProperty("Errors", response.Errors))
                {
                    logger.LogError("Request {RequestName} failed with errors", requestName);
                }
            }

            return response;
        }
    }
}

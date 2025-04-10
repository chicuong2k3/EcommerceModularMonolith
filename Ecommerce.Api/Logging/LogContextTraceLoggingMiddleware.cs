using Serilog.Context;
using System.Diagnostics;

namespace Ecommerce.Api.Logging;

internal class LogContextTraceLoggingMiddleware
{
    private readonly RequestDelegate next;

    public LogContextTraceLoggingMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var traceId = Activity.Current?.TraceId.ToString();
        var requestPath = context.Request.Path;
        using (LogContext.PushProperty("TraceId", traceId))
        using (LogContext.PushProperty("RequestPath", requestPath))
        {
            await next.Invoke(context);
        }
    }
}

internal static class LogContextTraceLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseLogContextTraceLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LogContextTraceLoggingMiddleware>();
    }
}
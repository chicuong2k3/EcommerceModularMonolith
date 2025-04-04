using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace Ecommerce.Api.Logging;

public static class Logging
{
    public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogger
    {
        get
        {
            return (context, loggerConfiguration) =>
            {
                var environment = context.HostingEnvironment;

                loggerConfiguration
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithProperty("Application", environment.ApplicationName)
                    .Enrich.WithProperty("Environment", environment.EnvironmentName)
                    .Enrich.WithExceptionDetails()
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
                    .WriteTo.Console();

                if (environment.IsDevelopment())
                {
                    loggerConfiguration.MinimumLevel.Override("Ecommerce.Api", LogEventLevel.Debug);
                }


            };
        }
    }
}

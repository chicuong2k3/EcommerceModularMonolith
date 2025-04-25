using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Reporting.Core;

public static class ReportingModule
{
    public static void AddReportingModule(this IServiceCollection services, IConfiguration configuration)
    {
    }

    //public static void ConfigureConsumers(this IRegistrationConfigurator registrationConfiguration)
    //{
    //}

    public static IApplicationBuilder UseReportingModule(this IApplicationBuilder app)
    {
        return app;
    }
}

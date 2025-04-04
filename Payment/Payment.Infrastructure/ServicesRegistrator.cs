using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Application;
using Payment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Payment.Infrastructure;

public static class ServicesRegistrator
{
    public static void RegisterPaymentServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Persistence
        var dbConnectionString = configuration.GetConnectionString("Database") ?? throw new InvalidOperationException("'Database' connection string cannot be null or empty.");

        //services.AddSingleton<DomainEventsToOutboxMessagesInterceptor>();
        services.AddDbContext<PaymentDbContext>((sp, options) =>
        {
            //var interceptor = sp.GetRequiredService<DomainEventsToOutboxMessagesInterceptor>();
            options.UseNpgsql(dbConnectionString);
            // .AddInterceptors(interceptor);
        });

        //services.AddScoped<IPaymentRepository, PaymentRepository>();

        // Add Health Checks
        services.AddHealthChecks()
            .AddNpgSql(dbConnectionString);

        // Add MediatR
        services.AddMediatR(configure =>
        {
            configure.RegisterServicesFromAssemblies([AssemblyInfo.Ref]);
        });


    }
}


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Shared.Infrastructure.Inbox;
using Ordering.Contracts;
using Pay.Core.Services;
using Pay.Core.Repositories;
using Pay.Core.Persistence;
using Pay.Core.Persistence.Repositories;

namespace Pay.Core;

public static class PayModule
{
    public static void AdPayModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Register repositories
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        // Register payment gateway
        services.AddScoped<IPaymentGatewayFactory, PaymentGatewayFactory>();
        services.AddScoped<IPaymentGateway, MomoGateway>();

        services.Configure<VNPayConfig>(configuration.GetSection("VNPayConfig"));
        services.AddScoped<IPaymentGateway, VNPayGateway>();
    }

    public static void ConfigureConsumers(this IRegistrationConfigurator registrationConfiguration)
    {
        registrationConfiguration.AddConsumer<IntegrationEventsToInboxMessagesConverter<OrderPlacedForOnlinePaymentIntegrationEvent, PayDbContext>>();
    }

    public static IApplicationBuilder UsePayModule(this IApplicationBuilder app)
    {
        app.ApplicationServices.MigratePayDatabaseAsync().Wait();
        return app;
    }
}


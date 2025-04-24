using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Pay.Infrastructure.Persistence;
using Pay.Infrastructure.Gateways;
using Pay.Infrastructure.Persistence.Repositories;
using Shared.Infrastructure.Inbox;
using Ordering.Contracts;
using Pay.Core;
using Pay.Core.Services;
using Pay.Core.Repositories;

namespace Pay.Infrastructure;

public static class PayModule
{
    public static void AdPayModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Register repositories
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        // Register payment gateway
        services.AddScoped<IPaymentGatewayFactory, PaymentGatewayFactory>();
        services.AddScoped<IPaymentGateway, MomoGateway>();
        services.AddSingleton<PaymentSettings>();
        services.Configure<PaymentSettings>(configuration.GetSection("PaymentSettings"));
    }

    public static void ConfigureConsumers(this IRegistrationConfigurator registrationConfiguration)
    {
        registrationConfiguration.AddConsumer<IntegrationEventsToInboxMessagesConverter<OrderPlacedForOnlinePaymentIntegrationEvent, PayDbContext>>();
    }

    public static IApplicationBuilder UseBillingModule(this IApplicationBuilder app)
    {
        return app;
    }
}


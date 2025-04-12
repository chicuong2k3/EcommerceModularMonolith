using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Billing.Infrastructure.Persistence.Repositories;
using Billing.Infrastructure.Gateways;
using Common.Infrastructure.Inbox;
using Billing.Infrastructure.Persistence;
using Ordering.Contracts;
using Billing.Domain.PaymentAggregate;
using Billing.Application;

namespace Billing.Infrastructure;

public static class ServicesRegistrator
{
    public static void RegisterBillingServices(this IServiceCollection services, IConfiguration configuration)
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
        registrationConfiguration.AddConsumer<IntegrationEventsToInboxMessagesConverter<OrderPlacedForOnlinePaymentIntegrationEvent, BillingDbContext>>();
    }
}


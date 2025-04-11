using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Billing.Infrastructure.Persistence.Repositories;
using Billing.Infrastructure.Gateways;
using Common.Infrastructure.Inbox;
using Billing.Infrastructure.Persistence;
using Ordering.Contracts;

namespace Billing.Infrastructure;

public static class ServicesRegistrator
{
    public static void RegisterPaymentServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register repositories
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        // Register payment gateway
        services.AddScoped<IPaymentGateway, MomoGateway>();
    }

    public static void ConfigureConsumers(this IRegistrationConfigurator registrationConfiguration)
    {
        registrationConfiguration.AddConsumer<IntegrationEventsToInboxMessagesConverter<OrderPlacedForOnlinePaymentIntegrationEvent, BillingDbContext>>();
    }
}


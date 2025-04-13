using Billing.Contracts;
using Catalog.Contracts;
using Common.Infrastructure.Inbox;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Persistence.Repositories;

namespace Ordering.Infrastructure;

public static class ServicesRegistrator
{
    public static void RegisterOrderingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICartRepository, CartRepository>();
        services.Decorate<ICartRepository, CachedCartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
    }

    public static void ConfigureConsumers(this IRegistrationConfigurator registrationConfiguration)
    {
        registrationConfiguration.AddConsumer<IntegrationEventsToInboxMessagesConverter<PaymentSucceededIntegrationEvent, OrderingDbContext>>();
        registrationConfiguration.AddConsumer<IntegrationEventsToInboxMessagesConverter<PaymentFailedIntegrationEvent, OrderingDbContext>>();
    }
}

using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Core.Persistence;
using Ordering.Core.Persistence.Repositories;
using Ordering.Core.Repositories;
using Pay.Contracts;
using Shared.Infrastructure.Inbox;

namespace Ordering.Core;

public static class OrderingModule
{
    public static void AddOrderingModule(this IServiceCollection services, IConfiguration configuration)
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

    public static IApplicationBuilder UseOrderingModule(this IApplicationBuilder app)
    {
        app.ApplicationServices.MigrateOrderingDatabaseAsync().Wait();
        return app;
    }
}

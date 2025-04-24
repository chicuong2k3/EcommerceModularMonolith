using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Core.Repositories;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Persistence.Repositories;
using Pay.Contracts;
using Shared.Infrastructure.Inbox;

namespace Ordering.Infrastructure;

public static class OrderingModule
{
    public static void AddOrderingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICartRepository, CartRepository>();
        services.Decorate<ICartRepository, CachedCartRepository>();
        services.AddScoped<IWriteOrderRepository, WriteOrderRepository>();
        services.AddScoped<IReadOrderRepository, ReadOrderRepository>();
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

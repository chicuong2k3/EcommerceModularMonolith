using Catalog.Contracts;
using Common.Infrastructure.Inbox;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.EventHandlers.IntegrationEvents;
using Ordering.Domain.ProductAggregate;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Persistence.Repositories;
using Ordering.Infrastructure.Services;

namespace Ordering.Infrastructure;

public static class ServicesRegistrator
{
    public static void RegisterOrderingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICartRepository, CartRepository>();
        services.Decorate<ICartRepository, CachedCartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICouponService, CouponClient>();

        services.AddHttpClient("PromotionService", client =>
        {
            var uri = configuration.GetConnectionString("PromotionService")
                ?? throw new ArgumentNullException("PromotionService Service Uri is not configured.");
            client.BaseAddress = new Uri(uri);
        });

    }

    public static void ConfigureConsumers(this IRegistrationConfigurator registrationConfiguration)
    {
        registrationConfiguration.AddConsumer<IntegrationEventsToInboxMessagesConverter<ProductCreatedIntegrationEvent, OrderingDbContext>>();
        registrationConfiguration.AddConsumer<IntegrationEventsToInboxMessagesConverter<ProductVariantAddedIntegrationEvent, OrderingDbContext>>();
    }
}

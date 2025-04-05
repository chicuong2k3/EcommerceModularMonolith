using Catalog.Application.EventHandlers.IntegrationEvents;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.Repositories;
using Common.Infrastructure.Inbox;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Contracts;

namespace Catalog.Infrastructure;

public static class ServicesRegistrator
{
    public static void RegisterCatalogServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
    }

    public static void ConfigureConsumers(this IRegistrationConfigurator registrationConfiguration)
    {
        registrationConfiguration.AddConsumer<IntegrationEventsToInboxMessagesConverter<OrderPlacedIntegrationEvent, CatalogDbContext>>();
    }


}


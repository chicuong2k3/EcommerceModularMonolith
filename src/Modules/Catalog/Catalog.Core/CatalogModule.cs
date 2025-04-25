using Catalog.Contracts;
using Catalog.Core.Persistence;
using Catalog.Core.Persistence.Repositories;
using Catalog.Core.Repositories;
using Catalog.Core.Services;
using Catalog.Infrastructure.Persistence;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Contracts;
using Shared.Infrastructure.Inbox;

namespace Catalog.Core;

public static class CatalogModule
{
    public static void AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();

        services.AddScoped<IProductService, ProductService>();
    }

    public static void ConfigureConsumers(this IRegistrationConfigurator registrationConfiguration)
    {
        registrationConfiguration.AddConsumer<IntegrationEventsToInboxMessagesConverter<OrderPlacedIntegrationEvent, CatalogDbContext>>();
    }

    public static IApplicationBuilder UseCatalogModule(this IApplicationBuilder app)
    {
        app.ApplicationServices.MigrateCatalogDatabaseAsync().Wait();

        return app;
    }

}


using Catalog.Contracts;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.Repositories;
using Catalog.Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Contracts;
using Shared.Infrastructure.Inbox;

namespace Catalog.Infrastructure;

public static class CatalogModule
{
    public static void AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IWriteCategoryRepository, WriteCategoryRepository>();
        services.AddScoped<IReadCategoryRepository, ReadCategoryRepository>();
        services.AddScoped<IWriteProductRepository, WriteProductRepository>();
        services.AddScoped<IReadProductRepository, ReadProductRepository>();
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


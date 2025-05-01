using Bogus;
using Catalog.Core.Persistence;
using MediatR;

namespace Catalog.IntegrationTests.Abstractions;

[Collection(nameof(IntegrationTestCollection))]
public class IntegrationTestBase : IDisposable
{
    protected readonly IServiceScope serviceScope;
    protected readonly IMediator mediator;
    protected readonly Faker faker = new();

    protected IntegrationTestBase(IntegrationTestWebAppFactory factory)
    {
        serviceScope = factory.Services.CreateScope();
        mediator = serviceScope.ServiceProvider.GetRequiredService<IMediator>();
        var db = serviceScope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        db.Database.EnsureCreated();
    }

    public void Dispose()
    {
        var db = serviceScope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        db.Database.EnsureDeleted();
        serviceScope.Dispose();
    }
}

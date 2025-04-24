using Bogus;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Ordering.IntegrationTests.Abstractions;

[Collection(nameof(IntegrationTestCollection))]
public class IntegrationTestBase : IDisposable
{
    protected IServiceScope serviceScope;
    protected IMediator mediator;
    protected readonly Faker faker = new();

    protected IntegrationTestBase(IntegrationTestWebAppFactory factory)
    {
        serviceScope = factory.Services.CreateScope();
        mediator = serviceScope.ServiceProvider.GetRequiredService<IMediator>();
    }

    // Helper method for test classes to create a new service scope with custom mocks
    protected void CreateNewServiceScope(IntegrationTestWebAppFactory factory, Action<IServiceCollection> configureMocks)
    {
        var customFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                configureMocks(services);
            });
        });

        serviceScope = customFactory.Services.CreateScope();
        mediator = serviceScope.ServiceProvider.GetRequiredService<IMediator>();
    }

    public void Dispose()
    {
        serviceScope.Dispose();
    }
}

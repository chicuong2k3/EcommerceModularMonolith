using Bogus;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Pay.IntegrationTests.Abstractions;

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
    }

    public void Dispose()
    {
        serviceScope.Dispose();
    }
}

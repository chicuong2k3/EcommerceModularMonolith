using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Catalog.IntegrationTests.Abstractions;

public class IntegrationTestWebAppFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer dbContainer
        = new PostgresSqlBuilder()
            .WithImage("postgres")
            .WithDatabase("ecommerce_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithCleanUp(true)
            .Build();

    private readonly RedisContainer redisContainer
        = new RedisBuilder()
            .WithImage("redis:latest")
            .WithCleanUp(true)
            .Build();

    public async Task InitializeAsync()
    {
        await dbContainer.StartAsync();
        await redisContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            // Add any additional configuration for the test environment here
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await dbContainer.StopAsync();
        await dbContainer.StopAsync();
    }
}

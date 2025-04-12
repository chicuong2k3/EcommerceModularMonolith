using Catalog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Billing.IntegrationTests.Abstractions;

public class IntegrationTestWebAppFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer dbContainer
        = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
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

    public new async Task DisposeAsync()
    {
        await dbContainer.DisposeAsync();
        await redisContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings:Database", dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ConnectionStrings:Cache", redisContainer.GetConnectionString());

        builder.ConfigureTestServices(services =>
        {
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<CatalogDbContext>();
            db.Database.Migrate();
        });
    }
}

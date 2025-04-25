using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Ordering.Core.Persistence;
using Quartz;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Ordering.IntegrationTests.Abstractions;

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
            // Remove Quartz hosted service for Outbox and Inbox consumers
            services.RemoveAll<IHostedService>();

            // Remove all Quartz job configurations
            services.RemoveAll<IScheduler>();
            services.RemoveAll<IJob>();

            // Remove all services with "ProcessOutboxMessagesJob" or "ProcessInboxMessagesJob" in their type name
            var descriptors = services.Where(d =>
                (d.ServiceType.FullName?.Contains("ProcessOutboxMessagesJob") == true) ||
                (d.ServiceType.FullName?.Contains("ProcessInboxMessagesJob") == true)).ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<OrderingDbContext>();
            db.Database.Migrate();
        });
    }
}

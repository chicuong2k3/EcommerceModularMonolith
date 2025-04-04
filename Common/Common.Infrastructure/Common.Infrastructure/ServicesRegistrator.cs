using Common.Application;
using Common.Infrastructure.Messaging;
using Common.Infrastructure.Outbox;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Quartz;
using System.Data;
using System.Reflection;

namespace Common.Infrastructure;

public static class ServicesRegistrator
{
    public static void RegisterCommonServices(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IRegistrationConfigurator>[] moduleConfigureConsumers,
        Assembly[] assemblies,
        params Type[] dbContextTypes)
    {
        // Add MediatR
        services.AddMediatR(configure =>
        {
            configure.RegisterServicesFromAssemblies(assemblies);
            configure.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
        });

        // Persistence & Outbox
        var dbConnectionString = configuration.GetConnectionString("Database") ?? throw new InvalidOperationException("'Database' connection string cannot be null or empty.");

        services.AddSingleton<DomainEventsToOutboxMessagesInterceptor>();
        var domainAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                                  .Where(a => a.FullName != null && a.FullName.Contains("Domain"));
        EventTypeRegistry.RegisterAllDomainEvents(domainAssemblies);

        var addDbContextMethod = typeof(EntityFrameworkServiceCollectionExtensions)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(m => m.Name == "AddDbContext"
                        && m.GetGenericArguments().Length == 1)
            .Where(m =>
            {
                var parms = m.GetParameters();
                return parms.Length == 4 &&
                       parms[0].ParameterType == typeof(IServiceCollection) &&
                       parms[1].ParameterType == typeof(Action<IServiceProvider, DbContextOptionsBuilder>) &&
                       parms[2].ParameterType == typeof(ServiceLifetime) &&
                       parms[3].ParameterType == typeof(ServiceLifetime);
            })
            .FirstOrDefault();
        if (addDbContextMethod == null)
        {
            throw new InvalidOperationException("Suitable AddDbContext overload not found.");
        }

        foreach (var dbContextType in dbContextTypes)
        {
            var genericAddDbContext = addDbContextMethod.MakeGenericMethod(dbContextType);
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction = (sp, options) =>
            {
                var interceptor = sp.GetRequiredService<DomainEventsToOutboxMessagesInterceptor>();
                options.UseNpgsql(dbConnectionString)
                       .AddInterceptors(interceptor);
            };
            var invokeParams = new object[]
            {
                services,
                optionsAction,
                ServiceLifetime.Scoped,
                ServiceLifetime.Singleton
            };
            genericAddDbContext.Invoke(null, invokeParams);


            var jobType = typeof(ProcessOutboxMessagesJob<>).MakeGenericType(dbContextType);
            var jobKey = new JobKey(jobType.Name);

            services.AddQuartz(configure =>
            {
                configure.AddJob(jobType, jobKey, jobBuilder => jobBuilder.StoreDurably())
                        .AddTrigger(trigger => trigger.ForJob(jobKey)
                            .WithSimpleSchedule(schedule => schedule
                                .WithIntervalInSeconds(10)
                                .RepeatForever()));

            });

        }

        services.AddQuartzHostedService();

        services.AddScoped<IDbConnection>(sp =>
        {
            var connection = new NpgsqlConnection(dbConnectionString);
            connection.Open();
            return connection;
        });

        // Caching
        var cacheConnectionString = configuration.GetConnectionString("Cache") ?? throw new ArgumentNullException("Cache Connection String is not configured.");
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = cacheConnectionString;
        });


        // Add Health Checks
        services.AddHealthChecks()
            .AddNpgSql(dbConnectionString)
            .AddRedis(cacheConnectionString);

        // MassTransit
        services.TryAddSingleton<IEventBus, EventBus>();
        services.AddMassTransit(x =>
        {
            x.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
            x.SetKebabCaseEndpointNameFormatter();

            foreach (var moduleConfigureConsumer in moduleConfigureConsumers)
            {
                moduleConfigureConsumer(x);
            }
        });

    }
}

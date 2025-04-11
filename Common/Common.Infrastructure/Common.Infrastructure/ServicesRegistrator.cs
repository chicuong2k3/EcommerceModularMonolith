using Common.Application;
using Common.Domain;
using Common.Infrastructure.Inbox;
using Common.Infrastructure.Messaging;
using Common.Infrastructure.Outbox;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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

        // Register Event Handlers
        services.AddDomainEventHandlers(assemblies, dbContextTypes);
        services.AddIntegrationEventHandlers(assemblies, dbContextTypes);

        // Persistence & Outbox & Inbox
        var dbConnectionString = configuration.GetConnectionString("Database") ?? throw new InvalidOperationException("'Database' connection string cannot be null or empty.");

        services.AddSingleton<DomainEventsToOutboxMessagesInterceptor>();
        var domainAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                                  .Where(a => a.FullName != null && a.FullName.Contains("Domain"));
        DomainEventTypeRegistry.RegisterAllDomainEvents(domainAssemblies);

        var contractsAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                                  .Where(a => a.FullName != null && a.FullName.Contains("Contracts"));
        IntegrationEventTypeRegistry.RegisterAllIntegrationEvents(contractsAssemblies);

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

            // Outbox
            var outboxJobType = typeof(ProcessOutboxMessagesJob<>).MakeGenericType(dbContextType);
            var outboxJobKey = new JobKey(outboxJobType.FullName ?? $"ProcessOutboxMessagesJob_{Guid.NewGuid()}");

            services.AddQuartz(configure =>
            {
                configure.AddJob(outboxJobType, outboxJobKey, jobBuilder => jobBuilder.StoreDurably())
                        .AddTrigger(trigger => trigger.ForJob(outboxJobKey)
                            .WithSimpleSchedule(schedule => schedule
                                .WithIntervalInSeconds(5)
                                .RepeatForever()));

            });

            services.AddScoped(outboxJobType, provider =>
            {
                var dbContext = provider.GetRequiredService(dbContextType);
                var loggerType = typeof(ILogger<>).MakeGenericType(outboxJobType);
                var logger = provider.GetRequiredService(loggerType);
                return Activator.CreateInstance(outboxJobType, dbContext, provider, assemblies, logger)
                    ?? throw new InvalidOperationException($"Failed to create instance of {outboxJobType.FullName}");
            });

            // Inbox
            var inboxJobType = typeof(ProcessInboxMessagesJob<>).MakeGenericType(dbContextType);
            var inboxJobKey = new JobKey(inboxJobType.FullName ?? $"ProcessInboxMessagesJob_{Guid.NewGuid()}");

            services.AddQuartz(configure =>
            {
                configure.AddJob(inboxJobType, inboxJobKey, jobBuilder => jobBuilder.StoreDurably())
                        .AddTrigger(trigger => trigger.ForJob(inboxJobKey)
                            .WithSimpleSchedule(schedule => schedule
                                .WithIntervalInSeconds(5)
                                .RepeatForever()));

            });

            services.AddScoped(inboxJobType, provider =>
            {
                var dbContext = provider.GetRequiredService(dbContextType);
                var loggerType = typeof(ILogger<>).MakeGenericType(inboxJobType);
                var logger = provider.GetRequiredService(loggerType);
                return Activator.CreateInstance(inboxJobType, dbContext, provider, assemblies, logger)
                    ?? throw new InvalidOperationException($"Failed to create instance of {inboxJobType.FullName}");
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


        // MassTransit

        services.TryAddSingleton<IEventBus, EventBus>();
        services.AddMassTransit(x =>
        {
            x.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });

            //var host = configuration["RabbitMq:Host"] ?? throw new ArgumentNullException("RabbitMq Host is not configured");
            //var username = configuration["RabbitMq:Username"] ?? throw new ArgumentNullException("RabbitMq Username is not configured");
            //var password = configuration["RabbitMq:Password"] ?? throw new ArgumentNullException("RabbitMq Password is not configured");
            //x.UsingRabbitMq((context, cfg) =>
            //{
            //    cfg.Host(new Uri(host), h =>
            //    {
            //        h.Username(username);
            //        h.Password(password);
            //    });
            //    cfg.ConfigureEndpoints(context);
            //});

            x.SetKebabCaseEndpointNameFormatter();

            //string instanceId = "ecommerce";
            foreach (var moduleConfigureConsumer in moduleConfigureConsumers)
            {
                moduleConfigureConsumer(x);
            }
        });

        // Add Health Checks
        services.AddHealthChecks()
            .AddNpgSql(dbConnectionString)
            .AddRedis(cacheConnectionString);


        // OpenTelemetry
        services.AddOpenTelemetry()
            .ConfigureResource(res => res.AddService("Ecommerce"))
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddNpgsql()
                    .AddRedisInstrumentation()
                    .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName)
                    .AddOtlpExporter();
            });

    }

    private static void AddDomainEventHandlers(
        this IServiceCollection services,
        Assembly[] assemblies,
        params Type[] dbContextTypes)
    {
        foreach (var assembly in assemblies)
        {
            var domainEventHandlerTypes = assembly
                .GetTypes()
                .Where(type => type.IsAssignableTo(typeof(IDomainEventHandler)))
                .ToList();

            foreach (var handlerType in domainEventHandlerTypes)
            {

                var interfaceType = handlerType
                    .GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));

                if (interfaceType == null)
                    continue;

                var dbContextType = ResolveDbContextForHandler(handlerType, dbContextTypes);

                if (dbContextType != null)
                {
                    var eventType = interfaceType.GetGenericArguments().Single();
                    var idempotentHandlerType = typeof(IdempotentDomainEventHandler<,>)
                        .MakeGenericType(eventType, dbContextType);

                    services.TryAddScoped(interfaceType, handlerType);
                    services.Decorate(interfaceType, idempotentHandlerType);
                }
            }
        }
    }

    private static void AddIntegrationEventHandlers(
        this IServiceCollection services,
        Assembly[] assemblies,
        params Type[] dbContextTypes)
    {
        foreach (var assembly in assemblies)
        {
            var integrationEventHandlerTypes = assembly
                .GetTypes()
                .Where(type => type.IsAssignableTo(typeof(IIntegrationEventHandler)))
                .ToList();

            foreach (var handlerType in integrationEventHandlerTypes)
            {

                var interfaceType = handlerType
                    .GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>));

                if (interfaceType == null)
                    continue;

                var dbContextType = ResolveDbContextForHandler(handlerType, dbContextTypes);

                if (dbContextType != null)
                {
                    var eventType = interfaceType.GetGenericArguments().Single();
                    var idempotentHandlerType = typeof(IdempotentIntegratonEventHandler<,>)
                        .MakeGenericType(eventType, dbContextType);

                    services.TryAddScoped(interfaceType, handlerType);
                    services.Decorate(interfaceType, idempotentHandlerType);
                }
            }
        }
    }

    private static Type? ResolveDbContextForHandler(Type handlerType, Type[] dbContextTypes)
    {
        var ns = handlerType.Namespace ?? string.Empty;
        var moduleName = ns.Split('.')[0];

        return dbContextTypes.FirstOrDefault(db => db.Namespace?.Contains(moduleName) == true);
    }
}

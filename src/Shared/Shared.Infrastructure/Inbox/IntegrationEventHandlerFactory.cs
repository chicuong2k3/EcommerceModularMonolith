using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions.Application;
using System.Collections.Concurrent;
using System.Reflection;

namespace Shared.Infrastructure.Inbox;

public static class IntegrationEventHandlerFactory
{
    private static readonly ConcurrentDictionary<string, Type[]> handlersDict = new();

    public static IEnumerable<IIntegrationEventHandler> GetHandlers(
        Type type,
        IServiceProvider serviceProvider,
        params Assembly[] assemblies)
    {

        var handlerTypes = handlersDict.GetOrAdd(
            type.FullName ?? throw new ArgumentException("Type has no full name.", nameof(type)),
            _ =>
            {
                var handlerInterfaceType = typeof(IIntegrationEventHandler<>).MakeGenericType(type);
                return assemblies
                    .SelectMany(a => a.GetTypes())
                    .Where(t => t.IsClass && !t.IsAbstract && handlerInterfaceType.IsAssignableFrom(t))
                    .Distinct()
                    .ToArray();
            });

        if (!handlerTypes.Any())
        {
            return Enumerable.Empty<IIntegrationEventHandler>();
        }

        var handlers = new List<IIntegrationEventHandler>();
        foreach (var handlerType in handlerTypes)
        {
            var handlerInterface = handlerType
                                        .GetInterfaces()
                                        .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>));

            if (handlerInterface == null)
            {
                throw new InvalidOperationException($"Handler '{handlerType.FullName}' does not implement IIntegrationEventHandler<>.");
            }

            var handler = serviceProvider.GetRequiredService(handlerInterface);
            if (handler is IIntegrationEventHandler integrationEventHandler)
            {
                handlers.Add(integrationEventHandler);
            }
            else
            {
                throw new InvalidOperationException($"Handler type '{handlerType.FullName}' does not implement IIntegrationEventHandler.");
            }
        }

        return handlers;
    }
}

using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions.Application;
using System.Collections.Concurrent;
using System.Reflection;

namespace Shared.Infrastructure.Outbox;

public static class DomainEventHandlerFactory
{
    private static readonly ConcurrentDictionary<string, Type[]> handlersDict = new();

    public static IEnumerable<IDomainEventHandler> GetHandlers(
        Type type,
        IServiceProvider serviceProvider,
        params Assembly[] assemblies)
    {

        var handlerTypes = handlersDict.GetOrAdd(
            type.FullName ?? throw new ArgumentException("Type has no full name.", nameof(type)),
            _ =>
            {
                var handlerInterfaceType = typeof(IDomainEventHandler<>).MakeGenericType(type);
                return assemblies
                    .SelectMany(a => a.GetTypes())
                    .Where(t => t.IsClass && !t.IsAbstract && handlerInterfaceType.IsAssignableFrom(t))
                    .Distinct()
                    .ToArray();
            });

        if (!handlerTypes.Any())
        {
            return Enumerable.Empty<IDomainEventHandler>();
        }

        var handlers = new List<IDomainEventHandler>();
        foreach (var handlerType in handlerTypes)
        {
            var handlerInterface = handlerType
                                        .GetInterfaces()
                                        .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));

            if (handlerInterface == null)
            {
                throw new InvalidOperationException($"Handler '{handlerType.FullName}' does not implement IDomainEventHandler<>.");
            }

            var handler = serviceProvider.GetRequiredService(handlerInterface);
            if (handler is IDomainEventHandler domainEventHandler)
            {
                handlers.Add(domainEventHandler);
            }
            else
            {
                throw new InvalidOperationException($"Handler type '{handlerType.FullName}' does not implement IDomainEventHandler.");
            }
        }

        return handlers;
    }
}

using Shared.Abstractions.Core;

namespace Catalog.Core.Events;

public record ProductCreated(Guid ProductId) : DomainEvent;
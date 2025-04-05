using Common.Messages;

namespace Catalog.Contracts;

public class ProductCreatedIntegrationEvent : IntegrationEvent
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
}

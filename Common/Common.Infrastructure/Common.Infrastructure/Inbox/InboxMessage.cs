using Common.Messages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Common.Infrastructure.Inbox;

public class InboxMessage
{
    [Key]
    public Guid Id { get; private set; }
    [Required]
    public string EventType { get; private set; }
    [Required]
    public string Content { get; private set; }
    public DateTime OccurredOn { get; private set; }
    public DateTime? ProcessedOn { get; set; }

    private InboxMessage()
    {
    }

    public InboxMessage(string type, object content)
    {
        Id = Guid.NewGuid();
        EventType = type ?? throw new ArgumentNullException(nameof(type));
        Content = JsonSerializer.Serialize(content);
        OccurredOn = DateTime.UtcNow;
    }

    public IntegrationEvent DeserializeContent()
    {
        var eventType = IntegrationEventTypeRegistry.Resolve(EventType)
            ?? throw new InvalidOperationException($"Event type '{EventType}' could not be resolved.");

        if (!typeof(IntegrationEvent).IsAssignableFrom(eventType))
        {
            throw new InvalidOperationException($"Resolved type '{eventType.FullName}' does not inherit from IntegrationEvent.");
        }

        var deserializedEvent = JsonSerializer.Deserialize(Content, eventType)
            ?? throw new InvalidOperationException($"Failed to deserialize content for event type '{EventType}'.");

        return (IntegrationEvent)deserializedEvent;
    }
}

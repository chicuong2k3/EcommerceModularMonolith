using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Common.Infrastructure.Outbox;

public class OutboxMessage
{
    [Key]
    public Guid Id { get; private set; }
    [Required]
    public string EventType { get; private set; }
    [Required]
    public string Content { get; private set; }
    public DateTime OccurredOn { get; private set; }
    public DateTime? ProcessedOn { get; set; }

    private OutboxMessage()
    {
    }

    public OutboxMessage(string type, object content)
    {
        Id = Guid.NewGuid();
        EventType = type ?? throw new ArgumentNullException(nameof(type));
        Content = JsonSerializer.Serialize(content);
        OccurredOn = DateTime.UtcNow;
    }

    public object DeserializeContent()
    {
        var eventType = EventTypeRegistry.Resolve(EventType);
        return JsonSerializer.Deserialize(Content, eventType)
               ?? throw new InvalidOperationException("Failed to deserialize outbox message content");
    }
}

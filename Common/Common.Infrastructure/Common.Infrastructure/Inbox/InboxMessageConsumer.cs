using System.ComponentModel.DataAnnotations;

namespace Common.Infrastructure.Inbox;

public class InboxMessageConsumer
{
    [Key]
    public Guid Id { get; private set; }
    public Guid InboxMessageId { get; private set; }
    [MaxLength(250)]
    public string Name { get; private set; }

    private InboxMessageConsumer() { } // For EF Core

    public InboxMessageConsumer(Guid outboxMessageId, string name)
    {
        Id = Guid.NewGuid();
        InboxMessageId = outboxMessageId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

}

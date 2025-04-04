using System.Text.Json.Serialization;

namespace Common.Domain;

public abstract class Entity
{
    [JsonInclude]
    public Guid Id { get; protected set; }
}

namespace Promotion.Domain.ConditionAggregate;

public class Condition : AggregateRoot
{
    public string Name { get; private set; }
    public ConditionType Type { get; private set; }
    public string Value { get; private set; }

    private Condition()
    {
    }

    private Condition(string name, ConditionType type, string value)
    {
        Id = Guid.NewGuid();
        Name = name;
        Type = type;
        Value = value.ToLower();
    }

    public static Result<Condition> Create(string name, ConditionType type, string value)
    {
        if (type == ConditionType.MinOrderTotal && !decimal.TryParse(value, out _))
        {
            return Result.Fail(new ValidationError("Invalid value for MinOrderTotal condition"));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Fail(new ValidationError("Condition name is required"));
        }

        var condition = new Condition(name, type, value);
        return Result.Ok(condition);
    }

    public bool IsSatisfied(OrderDetails orderDetails)
    {
        return Type switch
        {
            ConditionType.MinOrderTotal => orderDetails.Subtotal >= decimal.Parse(Value),
            _ => false
        };
    }
}
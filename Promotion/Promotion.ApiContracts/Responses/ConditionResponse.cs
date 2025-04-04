namespace Promotion.ApiContracts.Responses;

public class ConditionResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ConditionType { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
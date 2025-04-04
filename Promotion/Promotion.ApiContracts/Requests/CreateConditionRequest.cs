namespace Promotion.ApiContracts.Requests;

public class CreateConditionRequest
{
    public string Name { get; set; }
    public string ConditionType { get; set; }
    public string Value { get; set; }
}
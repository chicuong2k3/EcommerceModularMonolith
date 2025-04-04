namespace Promotion.ApiContracts.Requests;

public class CreateCouponRequest
{
    public string Code { get; set; }
    public string DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int UsageLimit { get; set; }
    public string? Description { get; set; }
    public List<Guid>? ConditionIds { get; set; }
}
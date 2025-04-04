namespace Promotion.ApiContracts.Responses;

public class CouponResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public string DiscountValue { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int UsageLimit { get; set; }
    public int CurrentUsageCount { get; set; }
    public string? Description { get; set; }
}

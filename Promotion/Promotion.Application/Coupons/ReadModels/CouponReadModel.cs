using Promotion.Application.Conditions.ReadModels;

namespace Promotion.Application.Coupons.ReadModels;

public class CouponReadModel
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public string DiscountValue { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int UsageLimit { get; set; }
    public int CurrentUsageCount { get; set; }
    public string? Description { get; set; }
    public List<ConditionReadModel> Conditions { get; set; } = new();
}
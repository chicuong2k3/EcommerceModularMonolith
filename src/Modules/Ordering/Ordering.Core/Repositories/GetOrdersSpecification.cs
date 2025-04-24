using Ordering.Core.ValueObjects;

namespace Ordering.Core.Repositories;

public class GetOrdersSpecification
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public Guid? CustomerId { get; set; }
    public OrderStatus? OrderStatus { get; set; }
    public DateTime? StartOrderDate { get; set; }
    public DateTime? EndOrderDate { get; set; }
    public decimal? MinTotal { get; set; }
    public decimal? MaxTotal { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
}

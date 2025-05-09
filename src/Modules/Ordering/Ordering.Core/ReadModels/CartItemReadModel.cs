﻿namespace Ordering.Core.ReadModels;

public class CartItemReadModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductVariantId { get; set; }
    public string ProductName { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal? SalePrice { get; set; }
    public int Quantity { get; set; }
    public string? Image { get; set; }
    public string? AttributesDescription { get; set; }
}
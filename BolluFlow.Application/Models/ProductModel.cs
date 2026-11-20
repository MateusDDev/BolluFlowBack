using BolluFlow.Domain.Entities;

namespace BolluFlow.Application.Models;

public class ProductModel
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Category { get; set; }
    public string? Description { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal StockQuantity { get; set; }
    public decimal? MinimumStockQuantity { get; set; }
    public decimal? IndirectCostPercentage { get; set; }
    public decimal MarkupPercentage { get; set; }
    public decimal FinalPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public bool TrackStock { get; set; }

    public static ProductModel From(Product product)
    {
        return new ProductModel
        {
            Id = product.Id,
            Name = product.Name,
            Category = product.Category,
            Description = product.Description,
            SalePrice = product.SalePrice,
            StockQuantity = product.StockQuantity,
            MinimumStockQuantity = product.MinimumStockQuantity,
            IndirectCostPercentage = product.IndirectCostPercentage,
            MarkupPercentage = product.MarkupPercentage,
            FinalPrice = product.FinalPrice,
            DiscountPercentage = product.DiscountPercentage,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive,
            TrackStock = product.TrackStock
        };
    }
}

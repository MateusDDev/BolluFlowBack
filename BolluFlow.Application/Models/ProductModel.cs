namespace BolluFlow.Application.Models;

public class ProductModel
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal? SalePrice { get; set; }
    public decimal StockQuantity { get; set; }
    public decimal? MinimumStockQuantity { get; set; }
    public decimal? IndirectCostPercentage { get; set; }
    public decimal MarkupPercentage { get; set; }
    public decimal FinalPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public bool IsActive { get; set; }
}

using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Models;

public class ProductComponentModel
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public ProductComponentType ComponentType { get; set; }
    public long? RecipeId { get; set; }
    public long? IngredientId { get; set; }
    public long? SupplyId { get; set; }
    public string Name { get; set; } = null!;
    public string BaseUnit { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public decimal StockQuantity { get; set; }
    public decimal QuantityUsed { get; set; }
}

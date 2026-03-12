using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Models;

public class SupplyModel
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public IngredientBaseUnit BaseUnit { get; set; }
    public decimal UnitCost { get; set; }
    public decimal StockQuantity { get; set; }
    public decimal? MinimumStockQuantity { get; set; }
}

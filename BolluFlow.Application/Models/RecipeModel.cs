using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Models;

public class RecipeModel
{
    public long Id { get; set; }

    public required string Name { get; set; }

    public required string Category { get; set; }

    public BaseUnit BaseUnit { get; set; }

    public int? PreparationTime { get; set; }

    public decimal UnitCost { get; set; }

    public decimal FinalPrice { get; set; }

    public decimal StockQuantity { get; set; }

    public decimal? MinimumStockQuantity { get; set; }

    public bool IsActive { get; set; }
}

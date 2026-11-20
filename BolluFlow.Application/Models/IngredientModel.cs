using BolluFlow.Domain.Entities;

namespace BolluFlow.Application.Models;

public class IngredientModel
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string BaseUnit { get; set; } = null!;

    public decimal UnitCost { get; set; }

    public decimal StockQuantity { get; set; }

    public decimal? MinimumStockQuantity { get; set; }

    public bool TrackStock { get; set; }

    public static IngredientModel From(Ingredient ingredient)
    {
        return new IngredientModel
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            BaseUnit = ingredient.BaseUnit.ToString(),
            UnitCost = ingredient.UnitCost,
            StockQuantity = ingredient.StockQuantity,
            MinimumStockQuantity = ingredient.MinimumStockQuantity,
            TrackStock = ingredient.TrackStock
        };
    }
}

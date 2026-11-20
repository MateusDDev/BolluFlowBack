using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Models;

public class SupplyModel
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public BaseUnit BaseUnit { get; set; }
    public decimal UnitCost { get; set; }
    public decimal StockQuantity { get; set; }
    public decimal? MinimumStockQuantity { get; set; }
    public bool TrackStock { get; set; }

    public static SupplyModel From(Supply supply)
    {
        return new SupplyModel
        {
            Id = supply.Id,
            Name = supply.Name,
            BaseUnit = supply.BaseUnit,
            UnitCost = supply.UnitCost,
            StockQuantity = supply.StockQuantity,
            MinimumStockQuantity = supply.MinimumStockQuantity,
            TrackStock = supply.TrackStock
        };
    }
}

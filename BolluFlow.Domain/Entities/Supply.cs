using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;

namespace BolluFlow.Domain.Entities;

public class Supply : IStockItem
{
    private Supply() { }

    public long Id { get; private set; }
    public long UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public BaseUnit BaseUnit { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal StockQuantity { get; private set; }
    public decimal? MinimumStockQuantity { get; private set; }
    public bool TrackStock { get; private set; }
    public List<SupplyConversion> SupplyConversions { get; init; } = [];

    public static Supply Create(long userId, string name, BaseUnit unit, decimal unitCost, decimal stockQuantity, decimal? minimumStockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(SupplyErrors.NameRequired);
        if (unitCost <= 0)
            throw new DomainException(SupplyErrors.UnitCostInvalid);
        if (stockQuantity < 0)
            throw new DomainException(SupplyErrors.StockQuantityInvalid);
        if (minimumStockQuantity < 0)
            throw new DomainException(SupplyErrors.MinimumStockQuantityInvalid);

        return new Supply
        {
            UserId = userId,
            Name = name,
            BaseUnit = unit,
            UnitCost = unitCost,
            StockQuantity = stockQuantity,
            MinimumStockQuantity = minimumStockQuantity,
            TrackStock = stockQuantity > 0
        };
    }

    public void Update(string name, BaseUnit unit, decimal unitCost, decimal stockQuantity, decimal? minimumStockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(SupplyErrors.NameRequired);
        if (unitCost <= 0)
            throw new DomainException(SupplyErrors.UnitCostInvalid);
        if (stockQuantity < 0)
            throw new DomainException(SupplyErrors.StockQuantityInvalid);
        if (minimumStockQuantity < 0)
            throw new DomainException(SupplyErrors.MinimumStockQuantityInvalid);

        Name = name;
        BaseUnit = unit;
        UnitCost = unitCost;
        StockQuantity = stockQuantity;
        MinimumStockQuantity = minimumStockQuantity;
    }

    public void UpdateUnitCost(decimal unitCost)
    {
        if (unitCost <= 0)
            throw new DomainException(SupplyErrors.UnitCostInvalid);

        UnitCost = unitCost;
    }

    public void SetTrackStock(bool trackStock)
    {
        TrackStock = trackStock;
    }

    public bool HasEnoughStock(decimal quantity, bool allowNegativeStock)
    {
        return !TrackStock || allowNegativeStock || StockQuantity >= quantity;
    }

    public void ConsumeStock(decimal quantity, bool allowNegativeStock = false)
    {
        if (quantity <= 0)
            throw new DomainException(SupplyErrors.StockQuantityInvalid);

        if (!TrackStock)
            return;

        if (!allowNegativeStock && StockQuantity < quantity)
            throw new DomainException(SupplyErrors.StockInsufficient(Name));

        StockQuantity -= quantity;
    }

    public void AddStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException(SupplyErrors.StockQuantityInvalid);

        TrackStock = true;
        StockQuantity += quantity;
    }

    public void RevertStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException(SupplyErrors.StockQuantityInvalid);

        if (!TrackStock)
            return;

        StockQuantity += quantity;
    }

    public void AddConversion(string name, decimal quantityInBaseUnit)
    {
        SupplyConversions.Add(SupplyConversion.Create(Id, name, quantityInBaseUnit));
    }
}

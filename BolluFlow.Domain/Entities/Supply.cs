using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;

namespace BolluFlow.Domain.Entities;

public class Supply
{
    private Supply() { }

    public long Id { get; private set; }
    public long UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public IngredientBaseUnit BaseUnit { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal StockQuantity { get; private set; }
    public decimal? MinimumStockQuantity { get; private set; }
    public List<SupplyConversion> SupplyConversions { get; init; } = [];

    public static Supply Create(long userId, string name, IngredientBaseUnit unit, decimal unitCost, decimal stockQuantity, decimal? minimumStockQuantity)
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
            MinimumStockQuantity = minimumStockQuantity
        };
    }

    public void Update(string name, IngredientBaseUnit unit, decimal unitCost, decimal stockQuantity, decimal? minimumStockQuantity)
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

    public void ConsumeStock(decimal quantity)
    {
        if (quantity <= 0 || StockQuantity < quantity)
            throw new DomainException(SupplyErrors.StockInsufficient(Name));

        StockQuantity -= quantity;
    }


    public void RevertStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException(SupplyErrors.StockQuantityInvalid);

        StockQuantity += quantity;
    }

    public void AddConversion(string name, decimal quantityInBaseUnit)
    {
        SupplyConversions.Add(SupplyConversion.Create(Id, name, quantityInBaseUnit));
    }
}

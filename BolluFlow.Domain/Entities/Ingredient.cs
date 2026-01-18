using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;

namespace BolluFlow.Domain.Entities;

public class Ingredient
{
    private Ingredient() { }

    public long Id { get; private set; }
    public long UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public IngredientBaseUnit BaseUnit { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal StockQuantity { get; private set; }
    public decimal? MinimumStockQuantity { get; private set; }
    public List<RecipeIngredient> RecipeIngredients { get; init; } = [];
    public List<IngredientConversion> IngredientConversions { get; init; } = [];

    public static Ingredient Create(
        long userId,
        string name,
        IngredientBaseUnit unit,
        decimal unitPrice,
        decimal stockQuantity,
        decimal? minimumStockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(IngredientErrors.NameRequired);

        if (unitPrice <= 0)
            throw new DomainException(IngredientErrors.UnitPriceInvalid);
        
        if (stockQuantity <= 0)
            throw new DomainException(IngredientErrors.QuantityInvalid);

        return new Ingredient
        {
            UserId = userId,
            Name = name,
            BaseUnit = unit,
            UnitCost = unitPrice,
            StockQuantity = stockQuantity,
            MinimumStockQuantity = minimumStockQuantity
        };
    }

    public void Update(
        string name, 
        IngredientBaseUnit unit, 
        decimal unitPrice,
        decimal stockQuantity,
        decimal? minimumStockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(IngredientErrors.NameRequired);

        if (unitPrice <= 0)
            throw new DomainException(IngredientErrors.UnitPriceInvalid);
        
        if (stockQuantity <= 0)
            throw new DomainException(IngredientErrors.QuantityInvalid);

        Name = name;
        BaseUnit = unit;
        UnitCost = unitPrice;
        StockQuantity = stockQuantity;
        MinimumStockQuantity = minimumStockQuantity;
    }
    
    public void ConsumeStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException(IngredientErrors.QuantityInvalid);

        if (StockQuantity < quantity) 
            throw new DomainException(IngredientErrors.StockInsufficient(Name));
        
        StockQuantity -= quantity;
    }
    
    public void RevertStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException(IngredientErrors.QuantityInvalid);

        StockQuantity += quantity;
    }

    public void AddConversion(string name, decimal quantityInBaseUnit)
    {
        IngredientConversions.Add(IngredientConversion.Create(Id, name, quantityInBaseUnit));
    }
}
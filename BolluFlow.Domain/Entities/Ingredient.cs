using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Errors;

namespace BolluFlow.Domain.Entities;

public class Ingredient
{
    private Ingredient() { }

    public long Id { get; private set; }
    public long UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Unit { get; private set; } = null!;
    public decimal UnitPrice { get; private set; }
    public decimal StockQuantity { get; private set; }
    public decimal? MinimumStockQuantity { get; private set; }
    public List<RecipeIngredient> RecipeIngredients { get; set; } = [];

    public static Ingredient Create(
        long userId,
        string name,
        string unit,
        decimal unitPrice,
        decimal stockQuantity,
        decimal? minimumStockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(IngredientErrors.NameRequired);

        if (string.IsNullOrWhiteSpace(unit))
            throw new DomainException(IngredientErrors.UnitRequired);

        if (unitPrice <= 0)
            throw new DomainException(IngredientErrors.UnitPriceInvalid);
        
        if (stockQuantity <= 0)
            throw new DomainException(IngredientErrors.QuantityInvalid);

        return new Ingredient
        {
            UserId = userId,
            Name = name,
            Unit = unit,
            UnitPrice = unitPrice,
            StockQuantity = stockQuantity,
            MinimumStockQuantity = minimumStockQuantity
        };
    }

    public void Update(
        string name, 
        string unit, 
        decimal unitPrice,
        decimal stockQuantity,
        decimal? minimumStockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(IngredientErrors.NameRequired);

        if (string.IsNullOrWhiteSpace(unit))
            throw new DomainException(IngredientErrors.UnitRequired);

        if (unitPrice <= 0)
            throw new DomainException(IngredientErrors.UnitPriceInvalid);
        
        if (stockQuantity <= 0)
            throw new DomainException(IngredientErrors.QuantityInvalid);

        Name = name;
        Unit = unit;
        UnitPrice = unitPrice;
        StockQuantity = stockQuantity;
        MinimumStockQuantity = minimumStockQuantity;
    }
    
    public void ConsumeStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException(IngredientErrors.QuantityInvalid);

        if (StockQuantity < quantity) 
            throw new DomainException(IngredientErrors.StockInsufficient);
        
        StockQuantity -= quantity;
    }
    
    public void AddStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException(IngredientErrors.QuantityInvalid);

        if (StockQuantity < quantity) 
            throw new DomainException(IngredientErrors.StockInsufficient);
        
        StockQuantity += quantity;
    }
}
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;
using BolluFlow.Domain.Services;

namespace BolluFlow.Domain.Entities;

public class Recipe
{
    private Recipe() { }

    public long Id { get; private set; }

    public long UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Category { get; set; } = null!;

    public int? PreparationTime { get; set; }

    public BaseUnit BaseUnit { get; private set; }

    public decimal FinalPrice { get; private set; }

    public decimal StockQuantity { get; private set; }

    public decimal? MinimumStockQuantity { get; private set; }

    public bool IsActive { get; set; }

    public List<RecipeIngredient> RecipeIngredients { get; set; } = [];

    public List<RecipeConversion> RecipeConversions { get; set; } = [];

    public static Recipe Create(
        long userId,
        string name,
        string category,
        BaseUnit baseUnit,
        int? preparationTime = null,
        decimal finalPrice = 0)
    {
        ValidateFinalPrice(finalPrice);

        return new Recipe
        {
            UserId = userId,
            Name = name,
            Category = category,
            BaseUnit = baseUnit,
            FinalPrice = finalPrice,
            StockQuantity = 0,
            IsActive = true,
            PreparationTime = preparationTime
        };
    }

    public void Update(
        string name,
        string category,
        BaseUnit baseUnit,
        int? preparationTime = null,
        bool isActive = true)
    {
        Name = name;
        Category = category;
        BaseUnit = baseUnit;
        IsActive = isActive;
        PreparationTime = preparationTime;
    }

    public void SetFinalPrice(decimal finalPrice)
    {
        ValidateFinalPrice(finalPrice);
        FinalPrice = finalPrice;
    }

    public void ProduceStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException(RecipeErrors.QuantityInvalid);

        StockQuantity += quantity;
    }

    public void ConsumeStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException(RecipeErrors.QuantityInvalid);

        if (StockQuantity < quantity)
            throw new DomainException(RecipeErrors.StockInsufficient(Name));

        StockQuantity -= quantity;
    }

    public void SetMinimumStockQuantity(decimal? minimumStockQuantity)
    {
        if (minimumStockQuantity < 0)
            throw new DomainException(RecipeErrors.MinimumStockQuantityInvalid);

        MinimumStockQuantity = minimumStockQuantity;
    }

    public void AddConversion(string name, decimal quantityInBaseUnit)
    {
        RecipeConversions.Add(RecipeConversion.Create(Id, name, quantityInBaseUnit));
    }

    public decimal CalculateUnitCost()
    {
        return RecipePricingService.CalculateIngredientCost(RecipeIngredients
            .Select(x => (UnitPrice: x.Ingredient.UnitCost, x.QuantityUsed))
            .ToList());
    }

    private static void ValidateFinalPrice(decimal finalPrice)
    {
        if (finalPrice < 0)
            throw new DomainException(RecipeErrors.FinalPriceInvalid);
    }
}

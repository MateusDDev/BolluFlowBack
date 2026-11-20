using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;
using BolluFlow.Domain.Services;

namespace BolluFlow.Domain.Entities;

public class Recipe : IStockItem
{
    private Recipe() { }

    public long Id { get; private set; }

    public long UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Category { get; set; } = null!;

    public int? PreparationTime { get; set; }

    public BaseUnit BaseUnit { get; private set; }

    public decimal? Yield { get; private set; }

    public decimal? CostPerBaseUnit => Yield is null or 0
        ? null
        : FinalPrice / Yield.Value;

    public decimal FinalPrice { get; private set; }

    public decimal StockQuantity { get; private set; }

    public decimal? MinimumStockQuantity { get; private set; }

    public bool IsActive { get; set; }

    public bool TrackStock { get; private set; }

    public List<RecipeIngredient> RecipeIngredients { get; set; } = [];

    public List<RecipeConversion> RecipeConversions { get; set; } = [];

    public static Recipe Create(
        long userId,
        string name,
        string category,
        BaseUnit baseUnit,
        decimal? yield,
        int? preparationTime = null,
        decimal finalPrice = 0)
    {
        ValidateFinalPrice(finalPrice);
        ValidateYield(yield);

        return new Recipe
        {
            UserId = userId,
            Name = name,
            Category = category,
            BaseUnit = baseUnit,
            Yield = yield,
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
        decimal? yield,
        int? preparationTime = null,
        bool isActive = true)
    {
        ValidateYield(yield);

        Name = name;
        Category = category;
        BaseUnit = baseUnit;
        Yield = yield;
        IsActive = isActive;
        PreparationTime = preparationTime;
    }

    public void SetFinalPrice(decimal finalPrice)
    {
        ValidateFinalPrice(finalPrice);
        FinalPrice = finalPrice;
    }

    public void SetTrackStock(bool trackStock)
    {
        TrackStock = trackStock;
    }

    public bool HasEnoughStock(decimal quantity, bool allowNegativeStock)
    {
        return !TrackStock || allowNegativeStock || StockQuantity >= quantity;
    }

    public void ProduceStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException(RecipeErrors.QuantityInvalid);

        TrackStock = true;
        StockQuantity += quantity;
    }

    public void RevertStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException(RecipeErrors.QuantityInvalid);

        if (!TrackStock)
            return;

        StockQuantity += quantity;
    }

    public void ConsumeStock(decimal quantity, bool allowNegativeStock = false)
    {
        if (quantity <= 0)
            throw new DomainException(RecipeErrors.QuantityInvalid);

        if (!TrackStock)
            return;

        if (!allowNegativeStock && StockQuantity < quantity)
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

    private static void ValidateYield(decimal? yield)
    {
        if (yield < 0)
            throw new DomainException(RecipeErrors.YieldInvalid);
    }
}

using BolluFlow.Domain.Abstractions;
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
    public decimal FinalPrice { get; private set; }
    public bool IsActive { get; set; }
    public string BaseUnit { get; private set; } = "unidade";
    public decimal StockQuantity { get; private set; }
    public List<RecipeIngredient> RecipeIngredients { get; set; } = [];
    public List<RecipeConversion> RecipeConversions { get; set; } = [];
    public List<RecipeStockMovement> RecipeStockMovements { get; set; } = [];

    public static Recipe Create(long userId, string name, string category, int? preparationTime = null, decimal finalPrice = 0, string? baseUnit = null, decimal stockQuantity = 0)
    {
        ValidateFinalPrice(finalPrice);
        ValidateBaseUnit(baseUnit ?? "unidade");
        ValidateStock(stockQuantity);

        return new Recipe
        {
            UserId = userId,
            Name = name,
            Category = category,
            FinalPrice = finalPrice,
            IsActive = true,
            PreparationTime = preparationTime,
            BaseUnit = string.IsNullOrWhiteSpace(baseUnit) ? "unidade" : baseUnit.Trim(),
            StockQuantity = stockQuantity
        };
    }

    public void Update(string name, string category, int? preparationTime = null, bool isActive = true, string? baseUnit = null)
    {
        ValidateBaseUnit(baseUnit ?? BaseUnit);
        Name = name;
        Category = category;
        IsActive = isActive;
        PreparationTime = preparationTime;
        BaseUnit = string.IsNullOrWhiteSpace(baseUnit) ? BaseUnit : baseUnit.Trim();
    }

    public void SetFinalPrice(decimal finalPrice)
    {
        ValidateFinalPrice(finalPrice);
        FinalPrice = finalPrice;
    }

    public void AddConversion(string name, decimal quantityInBaseUnit)
    {
        RecipeConversions.Add(RecipeConversion.Create(Id, name, quantityInBaseUnit));
    }

    public void Produce(long? conversionId, decimal quantity, DateOnly productionDate)
    {
        if (quantity <= 0)
            throw new DomainException(RecipeErrors.ProductionQuantityInvalid);

        decimal quantityInBaseUnit;
        if (conversionId is null)
        {
            quantityInBaseUnit = quantity;
        }
        else
        {
            var conversion = RecipeConversions.FirstOrDefault(x => x.Id == conversionId.Value)
                ?? throw new DomainException(RecipeErrors.ConversionNotFound);
            quantityInBaseUnit = conversion.QuantityInBaseUnit * quantity;
        }

        StockQuantity += quantityInBaseUnit;
        RecipeStockMovements.Add(RecipeStockMovement.Create(Id, conversionId, quantity, quantityInBaseUnit, productionDate));
    }

    public decimal CalculateUnitCost()
    {
        return RecipePricingService.CalculateIngredientCost(RecipeIngredients.Select(x => (UnitPrice: x.Ingredient.UnitCost, x.QuantityUsed)).ToList());
    }

    private static void ValidateFinalPrice(decimal finalPrice)
    {
        if (finalPrice < 0)
            throw new DomainException(RecipeErrors.FinalPriceInvalid);
    }

    private static void ValidateBaseUnit(string baseUnit)
    {
        if (string.IsNullOrWhiteSpace(baseUnit))
            throw new DomainException(RecipeErrors.BaseUnitRequired);
    }

    private static void ValidateStock(decimal stockQuantity)
    {
        if (stockQuantity < 0)
            throw new DomainException(RecipeErrors.StockInvalid);
    }
}

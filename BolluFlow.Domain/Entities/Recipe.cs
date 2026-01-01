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
    
    public decimal IndirectCostPercentage { get; set; }
    
    public decimal MarkupPercentage { get; set; }
    
    public decimal FinalPrice { get; set; }
    
    public decimal? DiscountPercentage { get; set; }
    
    public bool IsActive { get; set; }

    public List<RecipeIngredient> RecipeIngredients { get; set; } = [];

    public static Recipe Create(
        long userId,
        string name,
        string category,
        int? preparationTime = null)
    {
        return new Recipe
        {
            UserId = userId,
            Name = name,
            Category = category,
            IndirectCostPercentage = 0,
            MarkupPercentage = 0,
            FinalPrice = 0,
            DiscountPercentage = null,
            IsActive = true,
            PreparationTime = preparationTime
        };
    }
    
    public void Update(
        string name,
        string category,
        int? preparationTime = null,
        bool isActive = true)
    {
        Name = name;
        Category = category;
        IsActive = isActive;
        PreparationTime = preparationTime;
    }

    public void UpdatePricing(
        decimal indirectCostPercentage,
        decimal markupPercentage,
        decimal finalPrice,
        decimal? discountPercentage = null)
    {
        if (discountPercentage is < 0 or > 100)
            throw new DomainException(RecipeErrors.InvalidDiscount);
        
        if (discountPercentage.HasValue)
            finalPrice -= discountPercentage.Value;
        
        IndirectCostPercentage = indirectCostPercentage;
        MarkupPercentage = markupPercentage;
        FinalPrice = finalPrice;
        DiscountPercentage = discountPercentage;
    }
    
    public decimal CalculateUnitCost()
    {
        var ingredientCost = RecipePricingService.CalculateIngredientCost(RecipeIngredients
            .Select(x => (x.Ingredient.UnitPrice, x.QuantityUsed))
            .ToList());

        return RecipePricingService.CalculateFinalCost(ingredientCost, IndirectCostPercentage);
    }
}
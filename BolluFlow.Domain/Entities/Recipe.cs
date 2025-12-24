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
        decimal indirectCostPercentage,
        decimal markupPercentage,
        decimal finalPrice,
        decimal? discountPercentage = null,
        int? preparationTime = null,
        bool isActive = true)
    {
        if (discountPercentage is < 0 or > 100)
            throw new DomainException(RecipeErrors.InvalidDiscount);
        
        if (discountPercentage.HasValue)
            finalPrice -= discountPercentage.Value;
        
        return new Recipe
        {
            UserId = userId,
            Name = name,
            Category = category,
            IndirectCostPercentage = indirectCostPercentage,
            MarkupPercentage = markupPercentage,
            FinalPrice = finalPrice,
            DiscountPercentage = discountPercentage,
            IsActive = isActive,
            PreparationTime = preparationTime
        };
    }
    
    public void Update(
        string name,
        string category,
        decimal indirectCostPercentage,
        decimal markupPercentage,
        decimal finalPrice,
        decimal? discountPercentage = null,
        int? preparationTime = null,
        bool isActive = true)
    {
        if (discountPercentage is < 0 or > 100)
            throw new DomainException(RecipeErrors.InvalidDiscount);
        
        if (discountPercentage.HasValue)
            finalPrice -= discountPercentage.Value;
        
        Name = name;
        Category = category;
        IndirectCostPercentage = indirectCostPercentage;
        MarkupPercentage = markupPercentage;
        FinalPrice = finalPrice;
        DiscountPercentage = discountPercentage;
        IsActive = isActive;
        PreparationTime = preparationTime;
    }
    
    public decimal CalculateUnitCost()
    {
        var ingredientCost = RecipePricingService.CalculateIngredientCost(RecipeIngredients
            .Select(x => (x.Ingredient.UnitPrice, x.QuantityUsed))
            .ToList());

        return RecipePricingService.CalculateFinalCost(ingredientCost, IndirectCostPercentage);
    }
}
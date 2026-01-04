namespace BolluFlow.Domain.Services;

public static class RecipePricingService
{
    public static decimal CalculateIngredientCost(List<(decimal unitPrice, decimal quantityUsed)> ingredients)
    {
        return ingredients.Sum(x => x.unitPrice * x.quantityUsed);
    }

    public static decimal CalculateFinalCost(decimal ingredientCost, decimal indirectCostPercentage)
    {
        return ingredientCost * (1 + indirectCostPercentage);
    }

    public static decimal CalculateSuggestedPrice(decimal finalCost, decimal markupPercentage)
    {
        return finalCost * markupPercentage;
    }

    public static decimal CalculateMargin(decimal finalCost, decimal suggestedPrice)
    {
        return (suggestedPrice - finalCost) / suggestedPrice * 100;
    }
}
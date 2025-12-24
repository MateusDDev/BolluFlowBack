namespace BolluFlow.Application.Features.Recipes.SimulatePricing;

public record SimulatePricingResult(
    decimal IngredientCost,
    decimal FinalCost,
    decimal SuggestedPrice,
    decimal MarginPercentage );
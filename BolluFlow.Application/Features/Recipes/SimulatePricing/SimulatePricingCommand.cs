using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Recipes.SimulatePricing;

public record SimulatePricingCommand(
    List<SimulatePricingCommandIngredient> Ingredients,
    decimal IndirectCostPercentage,
    decimal MarkupPercentage) : ICommand<ResultOf<SimulatePricingResult>>;

public record SimulatePricingCommandIngredient(
    long IngredientId,
    int QuantityUsed);
using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Recipes.UpdateRecipePricing;

public record UpdateRecipePricingCommand(long RecipeId, UpdateRecipePricingCommandBody Body) : ICommand<Result>;

public record UpdateRecipePricingCommandBody(
    decimal IndirectCostPercentage,
    decimal MarkupPercentage,
    decimal FinalPrice,
    decimal? DiscountPercentage);
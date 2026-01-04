using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Recipes.Create;

public record CreateRecipeCommand(
    string Name,
    string Category,
    int? PreparationTime,
    decimal IndirectCostPercentage,
    decimal MarkupPercentage,
    decimal FinalPrice,
    decimal? DiscountPercentage) : ICommand<ResultOf<CreateRecipeResult>>;
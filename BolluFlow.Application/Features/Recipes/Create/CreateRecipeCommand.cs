using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Recipes.Create;

public record CreateRecipeCommand(
    string Name,
    string Category,
    int? PreparationTime,
    decimal? FinalPrice) : ICommand<ResultOf<CreateRecipeResult>>;

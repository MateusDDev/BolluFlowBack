using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Recipes.Create;

public record CreateRecipeCommand(
    string Name,
    string Category,
    BaseUnit BaseUnit,
    decimal? Yield,
    int? PreparationTime,
    decimal? FinalPrice) : ICommand<ResultOf<CreateRecipeResult>>;

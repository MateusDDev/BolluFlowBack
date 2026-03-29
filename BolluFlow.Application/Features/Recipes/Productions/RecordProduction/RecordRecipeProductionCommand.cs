using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Recipes.Productions.RecordProduction;

public record RecordRecipeProductionCommand(long RecipeId, RecordRecipeProductionCommandBody Body) : ICommand<ResultOf<RecipeProductionModel>>;

public record RecordRecipeProductionCommandBody(decimal Quantity, string? Notes);

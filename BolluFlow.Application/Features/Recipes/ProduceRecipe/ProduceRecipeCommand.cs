using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Recipes.ProduceRecipe;

public record ProduceRecipeCommand(long RecipeId, long? ConversionId, decimal Quantity, DateOnly ProductionDate) : ICommand<ResultOf<RecipeModel>>;

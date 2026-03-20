using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Recipes.Conversions.CreateRecipeConversion;

namespace BolluFlow.Application.Features.Recipes.Conversions.UpdateRecipeConversion;

public record UpdateRecipeConversionCommand(long RecipeId, long ConversionId, RecipeConversionBody Body) : ICommand<Result>;

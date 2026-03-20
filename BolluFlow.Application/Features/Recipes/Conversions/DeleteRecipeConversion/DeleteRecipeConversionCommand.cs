using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Recipes.Conversions.DeleteRecipeConversion;

public record DeleteRecipeConversionCommand(long RecipeId, long ConversionId) : ICommand<Result>;

using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Recipes.UnitConversion.DeleteConversion;

public record DeleteRecipeConversionCommand(long RecipeId, long ConversionId) : ICommand<Result>;

using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Ingredients.UnitConversion.DeleteConversion;

public record DeleteConversionCommand(long IngredientId, long ConversionId) : ICommand<Result>;
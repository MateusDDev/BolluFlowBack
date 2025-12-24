using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Ingredients.DeleteIngredient;

public record DeleteIngredientCommand(long IngredientId) : ICommand<Result>;
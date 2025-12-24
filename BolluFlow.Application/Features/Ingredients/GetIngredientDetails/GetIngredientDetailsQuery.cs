using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Ingredients.GetIngredientDetails;

public record GetIngredientDetailsQuery(long IngredientId) : IQuery<ResultOf<IngredientModel>>;
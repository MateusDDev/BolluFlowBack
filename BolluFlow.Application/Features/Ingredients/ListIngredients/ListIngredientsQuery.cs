using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Ingredients.ListIngredients;

public class ListIngredientsQuery : IQuery<ResultOf<List<IngredientModel>>>;
using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Dashboard.GetLowStockIngredients;

public record GetLowStockIngredientsQuery : IQuery<ResultOf<List<GetLowStockIngredientsResult>>>;
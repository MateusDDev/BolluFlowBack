using BolluFlow.Application.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Dashboard.GetLowStockIngredients;

public class GetLowStockIngredientsHandler(ApplicationDbContext db, CurrentUser currentUser)
    : IRequestHandler<GetLowStockIngredientsQuery, ResultOf<List<GetLowStockIngredientsResult>>>
{
    public async Task<ResultOf<List<GetLowStockIngredientsResult>>> Handle(GetLowStockIngredientsQuery request, CancellationToken cancellationToken)
    {
        var ingredients = await db.Ingredients
            .Where(x => x.StockQuantity < x.MinimumStockQuantity && currentUser.Id == x.UserId)
            .Take(10)
            .ToListAsync(cancellationToken);

        return ResultOf<List<GetLowStockIngredientsResult>>.Success(ingredients.Adapt<List<GetLowStockIngredientsResult>>());
    }
}
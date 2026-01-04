using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Ingredients.ListIngredients;

public class ListIngredientsHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<ListIngredientsQuery, ResultOf<List<IngredientModel>>>
{
    public async Task<ResultOf<List<IngredientModel>>> Handle(ListIngredientsQuery request, CancellationToken cancellationToken)
    {
        var ingredients = await db.Ingredients
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);

        return ResultOf<List<IngredientModel>>.Success(ingredients.Adapt<List<IngredientModel>>());
    }
}
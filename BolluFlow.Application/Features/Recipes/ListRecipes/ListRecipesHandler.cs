using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.ListRecipes;

public class ListRecipesHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<ListRecipesQuery, ResultOf<List<RecipeModel>>>
{
    public async Task<ResultOf<List<RecipeModel>>> Handle(ListRecipesQuery request, CancellationToken cancellationToken)
    {
        var recipes = await db.Recipes
            .AsNoTracking()
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);

        return ResultOf<List<RecipeModel>>.Success(recipes.Adapt<List<RecipeModel>>());
    }
}
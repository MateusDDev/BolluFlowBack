using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.GetRecipeDetails;

public class GetRecipeDetailsHandler(ApplicationDbContext db) : IRequestHandler<GetRecipeDetailsQuery, ResultOf<RecipeModel>>
{
    public async Task<ResultOf<RecipeModel>> Handle(GetRecipeDetailsQuery request, CancellationToken cancellationToken)
    {
        var recipe = await db.Recipes
            .FirstOrDefaultAsync(x => x.Id == request.RecipeId, cancellationToken);

        if (recipe is null)
            return ResultOf<RecipeModel>.BadRequest(CommonErrors.RecipeNotFound);

        return ResultOf<RecipeModel>.Success(recipe.Adapt<RecipeModel>());
    }
}
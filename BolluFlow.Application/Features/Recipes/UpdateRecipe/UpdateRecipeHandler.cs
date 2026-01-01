using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.UpdateRecipe;

public class UpdateRecipeHandler(ApplicationDbContext db) : IRequestHandler<UpdateRecipeCommand, ResultOf<RecipeModel>>
{
    public async Task<ResultOf<RecipeModel>> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        var command = request.Body;
        
        var recipe = await db.Recipes
            .FirstOrDefaultAsync(x => x.Id == request.RecipeId, cancellationToken);

        if (recipe is null)
            return ResultOf<RecipeModel>.BadRequest(CommonErrors.RecipeNotFound);
        
        recipe.Update(
            name: command.Name,
            category: command.Category,
            preparationTime: command.PreparationTime,
            isActive: command.IsActive);

        await db.SaveChangesAsync(cancellationToken);

        return ResultOf<RecipeModel>.Success(recipe.Adapt<RecipeModel>());
    }
}
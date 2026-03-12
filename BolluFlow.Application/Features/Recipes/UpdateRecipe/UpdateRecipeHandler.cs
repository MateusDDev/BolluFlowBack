using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.UpdateRecipe;

public class UpdateRecipeHandler(ApplicationDbContext db) : IRequestHandler<UpdateRecipeCommand, ResultOf<RecipeModel>>
{
    public async Task<ResultOf<RecipeModel>> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        var command = request.Body;

        var recipe = await db.Recipes
            .Include(x => x.RecipeIngredients)
            .ThenInclude(x => x.Ingredient)
            .FirstOrDefaultAsync(x => x.Id == request.RecipeId, cancellationToken);

        if (recipe is null)
            return ResultOf<RecipeModel>.BadRequest(CommonErrors.RecipeNotFound);

        recipe.Update(
            name: command.Name,
            category: command.Category,
            preparationTime: command.PreparationTime,
            isActive: command.IsActive);

        if (command.FinalPrice.HasValue)
            recipe.SetFinalPrice(command.FinalPrice.Value);

        await db.SaveChangesAsync(cancellationToken);

        return ResultOf<RecipeModel>.Success(new RecipeModel
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Category = recipe.Category,
            PreparationTime = recipe.PreparationTime,
            UnitCost = recipe.CalculateUnitCost(),
            FinalPrice = recipe.FinalPrice,
            IsActive = recipe.IsActive
        });
    }
}

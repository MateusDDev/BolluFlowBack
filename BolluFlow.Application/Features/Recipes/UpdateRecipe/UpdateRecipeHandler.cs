using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.UpdateRecipe;

public class UpdateRecipeHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<UpdateRecipeCommand, ResultOf<RecipeModel>>
{
    public async Task<ResultOf<RecipeModel>> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recipe = await db.Recipes.Include(x => x.RecipeIngredients).ThenInclude(x => x.Ingredient)
                .FirstOrDefaultAsync(x => x.Id == request.RecipeId && x.UserId == user.Id, cancellationToken);

            if (recipe is null)
                return ResultOf<RecipeModel>.BadRequest(CommonErrors.RecipeNotFound);

            recipe.Update(request.Body.Name, request.Body.Category, request.Body.PreparationTime, request.Body.IsActive, request.Body.BaseUnit);
            if (request.Body.FinalPrice is not null)
                recipe.SetFinalPrice(request.Body.FinalPrice.Value);

            await db.SaveChangesAsync(cancellationToken);

            return ResultOf<RecipeModel>.Success(new RecipeModel
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Category = recipe.Category,
                PreparationTime = recipe.PreparationTime,
                UnitCost = recipe.CalculateUnitCost(),
                FinalPrice = recipe.FinalPrice,
                IsActive = recipe.IsActive,
                BaseUnit = recipe.BaseUnit,
                StockQuantity = recipe.StockQuantity
            });
        }
        catch (DomainException e)
        {
            return ResultOf<RecipeModel>.BadRequest(e.Error);
        }
    }
}

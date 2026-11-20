using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.UpdateRecipe;

public class UpdateRecipeHandler(ApplicationDbContext db) : IRequestHandler<UpdateRecipeCommand, ResultOf<RecipeModel>>
{
    public async Task<ResultOf<RecipeModel>> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        var command = request.Body;

        if (!Enum.IsDefined(typeof(BaseUnit), command.BaseUnit))
            return ResultOf<RecipeModel>.BadRequest(new DomainError("RECIPE_BASE_UNIT_INVALID", "baseUnit inválida"));

        var recipe = await db.Recipes
            .Include(x => x.RecipeIngredients)
            .ThenInclude(x => x.Ingredient)
            .FirstOrDefaultAsync(x => x.Id == request.RecipeId, cancellationToken);

        if (recipe is null)
            return ResultOf<RecipeModel>.BadRequest(CommonErrors.RecipeNotFound);

        recipe.Update(
            name: command.Name,
            category: command.Category,
            baseUnit: command.BaseUnit,
            yield: command.Yield,
            preparationTime: command.PreparationTime,
            isActive: command.IsActive);

        if (command.FinalPrice.HasValue)
            recipe.SetFinalPrice(command.FinalPrice.Value);

        if (command.TrackStock.HasValue)
            recipe.SetTrackStock(command.TrackStock.Value);

        if (command.MinimumStockQuantity.IsSet)
            recipe.SetMinimumStockQuantity(command.MinimumStockQuantity.Value);

        await db.SaveChangesAsync(cancellationToken);

        return ResultOf<RecipeModel>.Success(new RecipeModel
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Category = recipe.Category,
            BaseUnit = recipe.BaseUnit,
            PreparationTime = recipe.PreparationTime,
            Yield = recipe.Yield,
            UnitCost = recipe.CalculateBatchCost(),
            CostPerBaseUnit = recipe.CostPerBaseUnit,
            FinalPrice = recipe.FinalPrice,
            StockQuantity = recipe.StockQuantity,
            MinimumStockQuantity = recipe.MinimumStockQuantity,
            IsActive = recipe.IsActive,
            TrackStock = recipe.TrackStock
        });
    }
}

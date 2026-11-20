using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.AdjustStock;

public class AdjustRecipeStockHandler(ApplicationDbContext db) : IRequestHandler<AdjustRecipeStockCommand, ResultOf<RecipeStockAdjustmentModel>>
{
    public async Task<ResultOf<RecipeStockAdjustmentModel>> Handle(AdjustRecipeStockCommand request, CancellationToken cancellationToken)
    {
        if (request.Body.Quantity <= 0)
            return ResultOf<RecipeStockAdjustmentModel>.BadRequest(new DomainError("RECIPE_STOCK_QUANTITY_INVALID", "quantity must be greater than zero"));

        if (string.IsNullOrWhiteSpace(request.Body.Reason) || request.Body.Reason.Trim().Length < 3)
            return ResultOf<RecipeStockAdjustmentModel>.BadRequest(new DomainError("RECIPE_STOCK_ADJUSTMENT_REASON_INVALID", "reason must have at least 3 characters"));

        var direction = request.Body.Direction.Trim().ToLowerInvariant();
        if (direction is not ("add" or "remove"))
            return ResultOf<RecipeStockAdjustmentModel>.BadRequest(new DomainError("RECIPE_STOCK_ADJUSTMENT_DIRECTION_INVALID", "direction must be 'add' or 'remove'"));

        try
        {
            var recipe = await db.Recipes.FirstOrDefaultAsync(x => x.Id == request.RecipeId, cancellationToken);
            if (recipe is null)
                return ResultOf<RecipeStockAdjustmentModel>.NotFound(CommonErrors.RecipeNotFound);

            recipe.SetTrackStock(true);

            if (direction == "add")
                recipe.ProduceStock(request.Body.Quantity);
            else
                recipe.ConsumeStock(request.Body.Quantity);

            await db.SaveChangesAsync(cancellationToken);

            return ResultOf<RecipeStockAdjustmentModel>.Success(new RecipeStockAdjustmentModel
            {
                RecipeId = recipe.Id,
                Quantity = request.Body.Quantity,
                Direction = direction,
                Reason = request.Body.Reason.Trim(),
                StockQuantity = recipe.StockQuantity,
                CreatedAt = DateTime.UtcNow
            });
        }
        catch (DomainException e)
        {
            return ResultOf<RecipeStockAdjustmentModel>.Unprocessable(e.Error);
        }
    }
}

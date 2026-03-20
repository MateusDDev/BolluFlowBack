using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.ProduceRecipe;

public class ProduceRecipeHandler(ApplicationDbContext db) : IRequestHandler<ProduceRecipeCommand, ResultOf<RecipeModel>>
{
    public async Task<ResultOf<RecipeModel>> Handle(ProduceRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recipe = await db.Recipes.Include(x => x.RecipeIngredients).ThenInclude(x => x.Ingredient).Include(x => x.RecipeConversions).FirstOrDefaultAsync(x => x.Id == request.RecipeId, cancellationToken);
            if (recipe is null)
                return ResultOf<RecipeModel>.NotFound(CommonErrors.RecipeNotFound);

            recipe.Produce(request.ConversionId, request.Quantity, request.ProductionDate);
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
        catch (DomainException ex)
        {
            return ResultOf<RecipeModel>.BadRequest(ex.Error);
        }
    }
}

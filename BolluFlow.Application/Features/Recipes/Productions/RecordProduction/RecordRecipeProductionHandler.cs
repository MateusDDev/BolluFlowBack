using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Application.Services;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.Productions.RecordProduction;

public class RecordRecipeProductionHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<RecordRecipeProductionCommand, ResultOf<RecipeProductionModel>>
{
    public async Task<ResultOf<RecipeProductionModel>> Handle(RecordRecipeProductionCommand request, CancellationToken cancellationToken)
    {
        if (request.Body.Quantity <= 0)
            return ResultOf<RecipeProductionModel>.BadRequest(new DomainError("RECIPE_PRODUCTION_QUANTITY_INVALID", "quantity deve ser maior que zero"));

        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var recipe = await db.Recipes
                .Include(x => x.RecipeIngredients)
                .ThenInclude(x => x.Ingredient)
                .FirstOrDefaultAsync(x => x.Id == request.RecipeId && x.UserId == user.Id, cancellationToken);

            if (recipe is null)
                return ResultOf<RecipeProductionModel>.NotFound(CommonErrors.RecipeNotFound);

            var policy = await StockPolicy.LoadAsync(db, user.Id, cancellationToken);

            var insufficientIngredients = recipe.RecipeIngredients
                .Select(x => new
                {
                    Ingredient = x.Ingredient,
                    Required = x.QuantityUsed * request.Body.Quantity
                })
                .Where(x => !policy.HasEnough(x.Ingredient, x.Required))
                .Select(x => new
                {
                    ingredientId = x.Ingredient.Id,
                    ingredientName = x.Ingredient.Name,
                    required = x.Required,
                    available = x.Ingredient.StockQuantity,
                    unit = x.Ingredient.BaseUnit.ToString().ToLowerInvariant()
                })
                .ToList();

            if (insufficientIngredients.Count > 0)
            {
                return ResultOf<RecipeProductionModel>.Unprocessable(
                    new DomainError("INSUFFICIENT_STOCK", "Estoque insuficiente para produzir esta receita"),
                    new { insufficientIngredients });
            }

            var production = RecipeProduction.Create(request.RecipeId, request.Body.Quantity, request.Body.Notes?.Trim());
            db.RecipeProductions.Add(production);
            await db.SaveChangesAsync(cancellationToken);

            var debits = new List<RecipeProductionDebitModel>();

            foreach (var recipeIngredient in recipe.RecipeIngredients)
            {
                var quantityToDebit = recipeIngredient.QuantityUsed * request.Body.Quantity;
                policy.Consume(recipeIngredient.Ingredient, quantityToDebit);

                var eventKey = $"recipe_production:{production.Id}:ingredient:{recipeIngredient.IngredientId}";
                var existingMovement = await db.StockMovements.FirstOrDefaultAsync(x => x.EventKey == eventKey, cancellationToken);

                if (policy.IsActive && existingMovement is null)
                {
                    db.StockMovements.Add(StockMovement.Create(
                        StockItemType.Ingredient,
                        recipeIngredient.IngredientId,
                        StockMovementType.Consumption,
                        quantityToDebit,
                        0,
                        $"Consumo - Produção de {recipe.Name}",
                        eventKey));
                }

                debits.Add(new RecipeProductionDebitModel
                {
                    IngredientId = recipeIngredient.IngredientId,
                    IngredientName = recipeIngredient.Ingredient.Name,
                    QuantityDebited = quantityToDebit,
                    Unit = recipeIngredient.Ingredient.BaseUnit.ToString().ToLowerInvariant()
                });
            }

            if (policy.IsActive)

                recipe.ProduceStock(request.Body.Quantity);

            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return ResultOf<RecipeProductionModel>.Success(new RecipeProductionModel
            {
                Id = production.Id,
                RecipeId = recipe.Id,
                Quantity = production.Quantity,
                Notes = production.Notes,
                Debits = debits,
                CreatedAt = production.CreatedAt
            });
        }
        catch (DomainException e)
        {
            await tx.RollbackAsync(cancellationToken);
            return ResultOf<RecipeProductionModel>.Unprocessable(e.Error);
        }
    }
}

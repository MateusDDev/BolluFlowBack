using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.Create;

public class CreateRecipeHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<CreateRecipeCommand, ResultOf<CreateRecipeResult>>
{
    public async Task<ResultOf<CreateRecipeResult>> Handle(
        CreateRecipeCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var ingredientIds = request.Ingredients
                .Select(x => x.IngredientId)
                .Distinct()
                .ToList();

            var ingredients = await db.Ingredients
                .Where(x => ingredientIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            if (ingredients.Count != ingredientIds.Count)
                return ResultOf<CreateRecipeResult>.NotFound(CommonErrors.IngredientNotFound);

            var recipe = Recipe.Create(
                userId: user.Id,
                name: request.Name,
                category: request.Category,
                indirectCostPercentage: request.IndirectCostPercentage,
                markupPercentage: request.MarkupPercentage,
                finalPrice: request.FinalPrice,
                discountPercentage: request.DiscountPercentage,
                preparationTime: request.PreparationTime);

            var recipeIngredients = request.Ingredients
                .Select(x => RecipeIngredient.Create(recipe, ingredients[x.IngredientId], x.QuantityUsed))
                .ToList();

            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
            
            db.Recipes.Add(recipe);
            
            await db.SaveChangesAsync(cancellationToken);

            db.RecipeIngredients.AddRange(recipeIngredients);
            
            await db.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return ResultOf<CreateRecipeResult>.Success(new CreateRecipeResult(recipe.Id));
        }
        catch (DomainException e)
        {
            return ResultOf<CreateRecipeResult>.BadRequest(e.Error);
        }
    }

}
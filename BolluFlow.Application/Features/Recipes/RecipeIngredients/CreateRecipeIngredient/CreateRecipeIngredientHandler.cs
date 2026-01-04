using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.RecipeIngredients.CreateRecipeIngredient;

public class CreateRecipeIngredientHandler(ApplicationDbContext db) : IRequestHandler<CreateRecipeIngredientCommand, Result>
{
    public async Task<Result> Handle(CreateRecipeIngredientCommand request, CancellationToken cancellationToken)
    {
        var command = request.Body;

        var recipeIngredientExists = await db.RecipeIngredients
            .AnyAsync(x => x.RecipeId == request.RecipeId && x.IngredientId == command.IngredientId, cancellationToken);

        if (recipeIngredientExists)
        {
            return Result.BadRequest(new DomainError(
                "RECIPE_INGREDIENT_ALREADY_EXISTIS",
                "Ingrediente já adicionado a receita"));
        }
        
        var recipeIngredient = RecipeIngredient.Create(request.RecipeId, command.IngredientId, command.QuantityUsed);
        
        db.RecipeIngredients.Add(recipeIngredient);
        
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}
using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Persistence;
using MediatR;

namespace BolluFlow.Application.Features.Recipes.RecipeIngredients.CreateRecipeIngredient;

public class CreateRecipeIngredientHandler(ApplicationDbContext db) : IRequestHandler<CreateRecipeIngredientCommand, Result>
{
    public async Task<Result> Handle(CreateRecipeIngredientCommand request, CancellationToken cancellationToken)
    {
        var command = request.Body;
        
        var recipeIngredient = RecipeIngredient.Create(request.RecipeId, command.IngredientId, command.QuantityUsed);
        
        db.RecipeIngredients.Add(recipeIngredient);
        
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}
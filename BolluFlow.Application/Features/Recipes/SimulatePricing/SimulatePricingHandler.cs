using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Services;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.SimulatePricing;

public class SimulatePricingHandler(ApplicationDbContext db) : IRequestHandler<SimulatePricingCommand, ResultOf<SimulatePricingResult>>
{
    public async Task<ResultOf<SimulatePricingResult>> Handle(SimulatePricingCommand request, CancellationToken cancellationToken)
    {
        var ingredientIds = request.Ingredients
            .Select(x => x.IngredientId)
            .Distinct()
            .ToList();

        var ingredients = await db.Ingredients
            .Where(x => ingredientIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (ingredients.Count != ingredientIds.Count)
        {
            return ResultOf<SimulatePricingResult>.BadRequest(new DomainError(
                "ONE_OR_MORE_INGREDIENTS_NOT_FOUND",
                "Um ou mais ingredientes não encontrados"));
        }
        
        var ingredientCost = RecipePricingService.CalculateIngredientCost(
            request.Ingredients.Select(i =>
            {
                var ingredient = ingredients
                    .First(x => x.Id == i.IngredientId);

                return (ingredient.UnitPrice, i.QuantityUsed);
            }).ToList());
        
        var finalCost = RecipePricingService.CalculateFinalCost(ingredientCost, request.IndirectCostPercentage);
        
        var suggestedPrice = RecipePricingService.CalculateSuggestedPrice(finalCost, request.MarkupPercentage);
        
        var margin = RecipePricingService.CalculateMargin(finalCost, suggestedPrice);

        return ResultOf<SimulatePricingResult>.Success(new SimulatePricingResult(
            ingredientCost, 
            finalCost,
            suggestedPrice,
            Math.Round(margin, 2)));
    }
}
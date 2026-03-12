using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.UpdateRecipePricing;

public class UpdateRecipePricingHandler(ApplicationDbContext db) : IRequestHandler<UpdateRecipePricingCommand, Result>
{
    public async Task<Result> Handle(UpdateRecipePricingCommand request, CancellationToken cancellationToken)
    {
        var recipe = await db.Recipes
            .FirstOrDefaultAsync(x => x.Id == request.RecipeId, cancellationToken);

        if (recipe is null)
            return Result.BadRequest(CommonErrors.RecipeNotFound);

        recipe.UpdatePricing(request.Body.IndirectCostPercentage);

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.Conversions.CreateRecipeConversion;

public class CreateRecipeConversionHandler(ApplicationDbContext db) : IRequestHandler<CreateRecipeConversionCommand, Result>
{
    public async Task<Result> Handle(CreateRecipeConversionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recipe = await db.Recipes.Include(x => x.RecipeConversions).FirstOrDefaultAsync(x => x.Id == request.RecipeId, cancellationToken);
            if (recipe is null)
                return Result.NotFound(CommonErrors.RecipeNotFound);

            recipe.AddConversion(request.Body.Name, request.Body.QuantityInBaseUnit);
            await db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.BadRequest(ex.Error);
        }
    }
}

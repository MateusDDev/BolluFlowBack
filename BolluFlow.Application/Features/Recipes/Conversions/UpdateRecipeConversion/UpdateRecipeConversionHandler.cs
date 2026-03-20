using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.Conversions.UpdateRecipeConversion;

public class UpdateRecipeConversionHandler(ApplicationDbContext db) : IRequestHandler<UpdateRecipeConversionCommand, Result>
{
    public async Task<Result> Handle(UpdateRecipeConversionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var conversion = await db.RecipeConversions.FirstOrDefaultAsync(x => x.RecipeId == request.RecipeId && x.Id == request.ConversionId, cancellationToken);
            if (conversion is null)
                return Result.NotFound(CommonErrors.RecipeNotFound);

            conversion.Update(request.Body.Name, request.Body.QuantityInBaseUnit);
            await db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.BadRequest(ex.Error);
        }
    }
}

using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Ingredients.UnitConversion.DeleteConversion;

public class DeleteConversionHandler(ApplicationDbContext db) : IRequestHandler<DeleteConversionCommand, Result>
{
    public async Task<Result> Handle(DeleteConversionCommand request, CancellationToken cancellationToken)
    {
        var ingredientConversion = await db.IngredientConversions
            .FirstOrDefaultAsync(x => x.IngredientId == request.IngredientId && x.Id == request.ConversionId, cancellationToken);

        if (ingredientConversion is null)
            return Result.NotFound(CommonErrors.IngredientNotFound);
        
        db.IngredientConversions.Remove(ingredientConversion);
        
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}
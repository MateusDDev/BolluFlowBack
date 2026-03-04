using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Ingredients.UnitConversion.UpdateConversion;

public class UpdateConversionHandler(ApplicationDbContext db) : IRequestHandler<UpdateConversionCommand, Result>
{
    public async Task<Result> Handle(UpdateConversionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = request.Body;

            var ingredientConversion = await db.IngredientConversions
                .FirstOrDefaultAsync(x => x.Id == request.ConversionId && 
                                          x.IngredientId == request.IngredientId, cancellationToken);

            if (ingredientConversion is null)
                return Result.NotFound(CommonErrors.IngredientConversionNotFound);

            ingredientConversion.Update(command.Name, command.QuantityInBaseUnit);

            await db.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException e)
        {
            return Result.BadRequest(e.Error);
        }
    }
}
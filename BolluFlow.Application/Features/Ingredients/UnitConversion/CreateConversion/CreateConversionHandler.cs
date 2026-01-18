using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Ingredients.UnitConversion.CreateConversion;

public class CreateConversionHandler(ApplicationDbContext db) : IRequestHandler<CreateConversionCommand, Result>
{
    public async Task<Result> Handle(CreateConversionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = request.Body;

            var ingredient = await db.Ingredients
                .FirstOrDefaultAsync(x => x.Id == request.IngredientId, cancellationToken);

            if (ingredient is null)
                return Result.NotFound(CommonErrors.IngredientNotFound);

            ingredient.AddConversion(command.Name, command.QuantityInBaseUnit);

            await db.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException e)
        {
            return Result.BadRequest(e.Error);
        }
    }
}
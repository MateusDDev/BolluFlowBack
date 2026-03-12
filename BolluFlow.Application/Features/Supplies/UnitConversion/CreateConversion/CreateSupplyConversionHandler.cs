using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Supplies.UnitConversion.CreateConversion;

public class CreateSupplyConversionHandler(ApplicationDbContext db) : IRequestHandler<CreateSupplyConversionCommand, Result>
{
    public async Task<Result> Handle(CreateSupplyConversionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var supply = await db.Supplies
                .Include(x => x.SupplyConversions)
                .FirstOrDefaultAsync(x => x.Id == request.SupplyId, cancellationToken);

            if (supply is null)
                return Result.NotFound(CommonErrors.SupplyNotFound);

            supply.AddConversion(request.Body.Name, request.Body.QuantityInBaseUnit);

            await db.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException e)
        {
            return Result.BadRequest(e.Error);
        }
    }
}

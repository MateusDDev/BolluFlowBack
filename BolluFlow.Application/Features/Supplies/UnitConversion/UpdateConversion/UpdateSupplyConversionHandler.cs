using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Supplies.UnitConversion.UpdateConversion;

public class UpdateSupplyConversionHandler(ApplicationDbContext db) : IRequestHandler<UpdateSupplyConversionCommand, Result>
{
    public async Task<Result> Handle(UpdateSupplyConversionCommand request, CancellationToken cancellationToken)
    {
        var conversion = await db.SupplyConversions
            .FirstOrDefaultAsync(x => x.SupplyId == request.SupplyId && x.Id == request.ConversionId, cancellationToken);

        if (conversion is null)
            return Result.NotFound(CommonErrors.SupplyConversionNotFound);

        conversion.Update(request.Body.Name, request.Body.QuantityInBaseUnit);

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

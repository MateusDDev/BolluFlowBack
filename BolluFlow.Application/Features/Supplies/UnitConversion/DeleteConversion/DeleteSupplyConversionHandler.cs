using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Supplies.UnitConversion.DeleteConversion;

public class DeleteSupplyConversionHandler(ApplicationDbContext db) : IRequestHandler<DeleteSupplyConversionCommand, Result>
{
    public async Task<Result> Handle(DeleteSupplyConversionCommand request, CancellationToken cancellationToken)
    {
        var conversion = await db.SupplyConversions
            .FirstOrDefaultAsync(x => x.SupplyId == request.SupplyId && x.Id == request.ConversionId, cancellationToken);

        if (conversion is null)
            return Result.NotFound(CommonErrors.SupplyConversionNotFound);

        db.SupplyConversions.Remove(conversion);
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

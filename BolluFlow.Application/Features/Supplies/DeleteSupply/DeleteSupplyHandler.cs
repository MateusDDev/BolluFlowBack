using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Supplies.DeleteSupply;

public class DeleteSupplyHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<DeleteSupplyCommand, Result>
{
    public async Task<Result> Handle(DeleteSupplyCommand request, CancellationToken cancellationToken)
    {
        var supply = await db.Supplies.FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == user.Id, cancellationToken);
        if (supply is null)
            return Result.NotFound(CommonErrors.SupplyNotFound);

        var inUse = await db.ProductComponents.AnyAsync(x => x.SupplyId == supply.Id, cancellationToken);

        if (inUse)
            return Result.BadRequest(new DomainError(
                "SUPPLY_IN_USE", "Este insumo está na ficha técnica de um produto. Remova de lá antes de excluir"));

        db.Supplies.Remove(supply);
        await db.SaveChangesAsync(cancellationToken);
        return Result.NoContent();
    }
}

using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
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

        db.Supplies.Remove(supply);
        await db.SaveChangesAsync(cancellationToken);
        return Result.NoContent();
    }
}

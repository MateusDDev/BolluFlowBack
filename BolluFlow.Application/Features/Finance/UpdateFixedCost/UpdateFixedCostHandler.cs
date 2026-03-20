using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Finance.Shared;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Finance.UpdateFixedCost;

public class UpdateFixedCostHandler(ApplicationDbContext db) : IRequestHandler<UpdateFixedCostCommand, ResultOf<FixedCostDto>>
{
    public async Task<ResultOf<FixedCostDto>> Handle(UpdateFixedCostCommand request, CancellationToken cancellationToken)
    {
        var item = await db.FixedCosts.FirstOrDefaultAsync(x => x.Id == request.FixedCostId && x.DeletedAt == null, cancellationToken);
        if (item is null)
            return ResultOf<FixedCostDto>.NotFound(FinanceErrors.FixedCostNotFound);
        if (request.Amount <= 0)
            return ResultOf<FixedCostDto>.BadRequest(FinanceErrors.InvalidAmount);
        if (!FinanceMonth.TryParseMonth(request.StartMonth, out _))
            return ResultOf<FixedCostDto>.BadRequest(FinanceErrors.InvalidMonth);
        if (request.EndMonth is not null && !FinanceMonth.TryParseMonth(request.EndMonth, out _))
            return ResultOf<FixedCostDto>.BadRequest(FinanceErrors.InvalidMonth);

        item.Update(request.Name, request.Amount, request.StartMonth, request.EndMonth, request.Notes);
        await db.SaveChangesAsync(cancellationToken);
        return ResultOf<FixedCostDto>.Success(item.ToDto());
    }
}

using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Finance.Shared;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Finance.DeleteFixedCost;

public class DeleteFixedCostHandler(ApplicationDbContext db) : IRequestHandler<DeleteFixedCostCommand, ResultOf<object>>
{
    public async Task<ResultOf<object>> Handle(DeleteFixedCostCommand request, CancellationToken cancellationToken)
    {
        if (!FinanceMonth.TryParseMonth(request.Month, out var monthDate))
            return ResultOf<object>.BadRequest(FinanceErrors.InvalidMonth);

        var item = await db.FixedCosts.FirstOrDefaultAsync(x => x.Id == request.FixedCostId && x.DeletedAt == null, cancellationToken);
        if (item is null)
            return ResultOf<object>.NotFound(FinanceErrors.FixedCostNotFound);

        switch (request.Mode)
        {
            case "until-this-month":
                item.SetEndMonth(request.Month);
                break;
            case "from-this-month-forward":
                var previousMonth = new DateOnly(monthDate.Year, monthDate.Month, 1).AddMonths(-1);
                item.SetEndMonth(FinanceMonth.Format(previousMonth));
                break;
            case "permanent":
                item.DeletePermanently();
                break;
            default:
                return ResultOf<object>.BadRequest(FinanceErrors.InvalidDeleteMode);
        }

        await db.SaveChangesAsync(cancellationToken);
        return ResultOf<object>.Success(new { deleted = true, mode = request.Mode, month = request.Month });
    }
}

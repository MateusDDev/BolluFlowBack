using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Finance.Shared;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Persistence;
using MediatR;

namespace BolluFlow.Application.Features.Finance.CreateFixedCost;

public class CreateFixedCostHandler(ApplicationDbContext db) : IRequestHandler<CreateFixedCostCommand, ResultOf<FixedCostDto>>
{
    public async Task<ResultOf<FixedCostDto>> Handle(CreateFixedCostCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
            return ResultOf<FixedCostDto>.BadRequest(FinanceErrors.InvalidAmount);
        if (!FinanceMonth.TryParseMonth(request.StartMonth, out _))
            return ResultOf<FixedCostDto>.BadRequest(FinanceErrors.InvalidMonth);
        if (request.EndMonth is not null && !FinanceMonth.TryParseMonth(request.EndMonth, out _))
            return ResultOf<FixedCostDto>.BadRequest(FinanceErrors.InvalidMonth);

        var item = FixedCost.Create(request.Name, request.Amount, request.StartMonth, request.EndMonth, request.Notes);
        db.FixedCosts.Add(item);
        await db.SaveChangesAsync(cancellationToken);

        return ResultOf<FixedCostDto>.Success(item.ToDto());
    }
}
